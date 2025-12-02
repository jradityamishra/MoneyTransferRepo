using Microsoft.EntityFrameworkCore;
using TransactionMicroservices.IServiceContracts;
using TransactionMicroservices.Model.DTO;
using TransactionMicroservices.Model.Entity;
using UserMicroservices.Data;
using TransactionMicroservices.Model;
using TransactionMicroservices.Model.Enums;
using TransactionMicroservices.Clients;
namespace TransactionMicroservices.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly DatabaseContext _context;
        private readonly GatewayClient _gatewayClient;
        public TransactionService(DatabaseContext db,GatewayClient gatewayClient)
        {
            _context = db;
            _gatewayClient = gatewayClient;
        }
        public async Task<TransactionResult> InitiateTransferAsync(TransactionRequest request)
        {
            // Basic validation (expand as needed)
            if (request == null ||
                string.IsNullOrWhiteSpace(request.FromAccountId) ||
                string.IsNullOrWhiteSpace(request.ToAccountId) ||
                request.Amount <= 0 ||
                string.IsNullOrWhiteSpace(request.Currency))
            {
                return new TransactionResult
                {
                    Success = false,
                    Message = "Invalid transfer request."
                };
            }
            
            //port to be tested
            bool fromAccountExists = await _gatewayClient.ValidateAccountExistsAsync(request.FromAccountId);
            bool toAccountExists = await _gatewayClient.ValidateAccountExistsAsync(request.ToAccountId);
            
            if(!fromAccountExists)
            {
                return new TransactionResult
                {
                    Success = false,
                    Message = "From Account does not exist."
                };
            }

            if (!toAccountExists)
            {
                return new TransactionResult
                {
                    Success = false,
                    Message = "To Account does not exist."
                };
            }
           
            bool balanceCheck = await _gatewayClient.CheckBalanceAsync(request.Amount, request.FromAccountId);
            if(!balanceCheck)
            {
                return new TransactionResult
                {
                    Success = false,
                    Message = "Insufficient balance."
                };
            }
            
            // Debit operation
            var updateFromAccount = await _gatewayClient.UpdateAccountBalanceAsync(request.FromAccountId, request.Amount, "debit");
            if(!updateFromAccount.Success)
            {
                return new TransactionResult
                {
                    Success = false,
                    Message = updateFromAccount.Message ?? "Failed to debit from account.",
                    DebitOperation = new OperationDetails
                    {
                        AccountNumber = request.FromAccountId,
                        OperationType = "Debit",
                        Amount = request.Amount,
                        Status = "Failed: " + updateFromAccount.Message
                    }
                };
            }
            
            bool fromAccountLocked = await _gatewayClient.LockAccountAsync(request.FromAccountId, "Lock");
            if (!fromAccountLocked)
            {
                return new TransactionResult
                {
                    Success = false,
                    Message = "Unable to lock the account for transaction."
                };
            }
            
            // Credit operation
            var updateToAccount = await _gatewayClient.UpdateAccountBalanceAsync(request.ToAccountId, request.Amount, "credit");
            if(!updateToAccount.Success)
            {
                // Rollback: unlock the from account
                await _gatewayClient.UnlockAccountAsync(request.FromAccountId, "Unlock");
                
                return new TransactionResult
                {
                    Success = false,
                    Message = updateToAccount.Message ?? "Failed to credit to account.",
                    DebitOperation = new OperationDetails
                    {
                        AccountNumber = request.FromAccountId,
                        OperationType = "Debit",
                        Amount = request.Amount,
                        NewBalance = updateFromAccount.Balance,
                        Status = "Completed (Rolled back due to credit failure)"
                    },
                    CreditOperation = new OperationDetails
                    {
                        AccountNumber = request.ToAccountId,
                        OperationType = "Credit",
                        Amount = request.Amount,
                        Status = "Failed: " + updateToAccount.Message
                    }
                };
            }
            
            var transaction = new TransactionSchema
            {
                Id = Guid.NewGuid(),
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = TransactionStatus.Completed,
                Type = TransactionType.Transfer,
                Description = request.Description ?? string.Empty,
                Reference = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                InitiatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
                FailureReason = null
            };

            _context.TransactionDB.Add(transaction);
            await _context.SaveChangesAsync();

            // Unlock the from account
            await _gatewayClient.UnlockAccountAsync(request.FromAccountId, "Unlock");

            // Map to DTO
            var dto = new TransactionDto
            {
                Id = transaction.Id,
                FromAccountId = transaction.FromAccountId,
                ToAccountId = transaction.ToAccountId,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Status = transaction.Status,
                Type = transaction.Type,
                Description = transaction.Description,
                Reference = transaction.Reference,
                InitiatedAt = transaction.InitiatedAt,
                CompletedAt = transaction.CompletedAt,
                FailureReason = transaction.FailureReason
            };

            bool fromAccountUnlocked = await _gatewayClient.LockAccountAsync(request.FromAccountId, "Unlock");
            if (!fromAccountUnlocked)
            {
                return new TransactionResult
                {
                    Success = false,
                    Message = "Unable to Unlock Your Account."
                };
            }


            return new TransactionResult
            {
                Success = true,
                Message = "Transaction completed successfully",
                Transaction = dto,
                DebitOperation = new OperationDetails
                {
                    AccountNumber = request.FromAccountId,
                    OperationType = "Debit",
                    Amount = request.Amount,
                    NewBalance = updateFromAccount.Balance,
                    Status = "Completed"
                },
                CreditOperation = new OperationDetails
                {
                    AccountNumber = request.ToAccountId,
                    OperationType = "Credit",
                    Amount = request.Amount,
                    NewBalance = updateToAccount.Balance,
                    Status = "Completed"
                }
            };
        }

        public async Task<TransactionDto> GetTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await _context.TransactionDB.FindAsync(transactionId);
            if (transaction == null)
            {
                return null;
            }
            // Map to DTO
            var dto = new TransactionDto
            {
                Id = transaction.Id,
                FromAccountId = transaction.FromAccountId,
                ToAccountId = transaction.ToAccountId,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Status = transaction.Status,
                Type = transaction.Type,
                Description = transaction.Description,
                Reference = transaction.Reference,
                InitiatedAt = transaction.InitiatedAt,
                CompletedAt = transaction.CompletedAt,
                FailureReason = transaction.FailureReason
            };
            return dto;
        }
        public async Task<IEnumerable<TransactionDto>> GetAccountTransactionsAsync(string accountId, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(accountId) || page <= 0 || pageSize <= 0)
            {
                return Enumerable.Empty<TransactionDto>();
            }

            var query = _context.TransactionDB
                .AsNoTracking()
                .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
                .OrderByDescending(t => t.InitiatedAt);

            var transactions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return transactions.Select(transaction => new TransactionDto
            {
                Id = transaction.Id,
                FromAccountId = transaction.FromAccountId,
                ToAccountId = transaction.ToAccountId,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Status = transaction.Status,
                Type = transaction.Type,
                Description = transaction.Description,
                Reference = transaction.Reference,
                InitiatedAt = transaction.InitiatedAt,
                CompletedAt = transaction.CompletedAt,
                FailureReason = transaction.FailureReason
            });
        }
        public async Task<IEnumerable<TransactionDto>> GetFilteredAccountTransactionAsync(string accountId, string operation, DateTime start, DateTime end)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return Enumerable.Empty<TransactionDto>();
            }

            var query = _context.TransactionDB
                .AsNoTracking()
                .Where(t => t.InitiatedAt >= start && t.InitiatedAt <= end);

            // Filter by operation type (debit or credit)
            if (!string.IsNullOrWhiteSpace(operation))
            {
                var op = operation.ToLower();
                if (op == "debit")
                {
                    // Debit means money going out from this account
                    query = query.Where(t => t.FromAccountId == accountId);
                }
                else if (op == "credit")
                {
                    // Credit means money coming into this account
                    query = query.Where(t => t.ToAccountId == accountId);
                }
                else
                {
                    // If operation is invalid or "all", show all transactions for the account
                    query = query.Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId);
                }
            }
            else
            {
                // If no operation specified, show all transactions for the account
                query = query.Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId);
            }

            var transactions = await query
                .OrderByDescending(t => t.InitiatedAt)
                .ToListAsync();

            return transactions.Select(transaction => new TransactionDto
            {
                Id = transaction.Id,
                FromAccountId = transaction.FromAccountId,
                ToAccountId = transaction.ToAccountId,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Status = transaction.Status,
                Type = transaction.Type,
                Description = transaction.Description,
                Reference = transaction.Reference,
                InitiatedAt = transaction.InitiatedAt,
                CompletedAt = transaction.CompletedAt,
                FailureReason = transaction.FailureReason
            });
        }
        public async Task<bool> CancelTransactionAsync(Guid transactionId)
        {
            var transaction = await _context.TransactionDB
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return false;

            // Only allow cancellation if not already completed, cancelled, failed, or reversed
            if (transaction.Status == TransactionStatus.Completed ||
                transaction.Status == TransactionStatus.Cancelled ||
                transaction.Status == TransactionStatus.Failed ||
                transaction.Status == TransactionStatus.Reversed)
            {
                return false;
            }

            transaction.Status = TransactionStatus.Cancelled;
            transaction.CompletedAt = DateTime.UtcNow;
            transaction.FailureReason = "Cancelled by user/request.";

            _context.TransactionDB.Update(transaction);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
