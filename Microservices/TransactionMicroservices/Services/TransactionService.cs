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
        public async Task<TransactionDto> InitiateTransferAsync(TransactionRequest request)
        {
            // Basic validation (expand as needed)
            if (request == null ||
                string.IsNullOrWhiteSpace(request.FromAccountId) ||
                string.IsNullOrWhiteSpace(request.ToAccountId) ||
                request.Amount <= 0 ||
                string.IsNullOrWhiteSpace(request.Currency))
            {
                throw new ArgumentException("Invalid transfer request.");
            }
            //port to be tested
            bool fromAccountExists = await _gatewayClient.ValidateAccountExistsAsync(request.FromAccountId);
            bool toAccountExists = await _gatewayClient.ValidateAccountExistsAsync(request.ToAccountId);
            if(fromAccountExists)
                {
                throw new ArgumentException("From Account does not exist.");
            }
            if (toAccountExists)
            {
                throw new ArgumentException("To Account does not exist.");
            }
            bool fromAccountLocked = await _gatewayClient.LockAccountAsync(request.FromAccountId);
            bool balanceCheck = await _gatewayClient.CheckBalanceAsync(request.Amount,request.FromAccountId);
            if(!balanceCheck)
            {
                throw new ArgumentException("Insufficient balance.");
            }
            bool updateFromAccount = await _gatewayClient.UpdateAccountBalanceAsync(request.FromAccountId,request.Amount, "debit");
            if(!updateFromAccount) {
                throw new ArgumentException("Failed to debit from account.");
            }
            bool updateToAccount = await _gatewayClient.UpdateAccountBalanceAsync(request.ToAccountId, request.Amount, "credit");
            if(!updateToAccount) {
                throw new ArgumentException("Failed to credit to account.");
            }
            var transaction = new TransactionSchema
            {
                Id = Guid.NewGuid(),
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = TransactionStatus.Initiated,
                Type = TransactionType.Transfer,
                Description = request.Description,
                InitiatedAt = DateTime.UtcNow,
                CompletedAt = null,
                FailureReason = null
            };

            _context.TransactionDB.Add(transaction);
            await _context.SaveChangesAsync();

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
        public async Task<IEnumerable<TransactionDto>> GetTransactionsByAccountIdAsync(string accountId, int page, int pageSize)
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
