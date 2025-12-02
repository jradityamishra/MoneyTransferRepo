using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionMicroservices.IServiceContracts;
using TransactionMicroservices.Model.DTO;

namespace TransactionMicroservices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class TransactionController : ControllerBase

    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        { 
          _transactionService = transactionService;
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<TransactionDto>> InitiateTransfer([FromBody] TransactionRequest request)
        {
            if (request == null || request.Amount <= 0 || string.IsNullOrEmpty(request.FromAccountId) || string.IsNullOrEmpty(request.ToAccountId))
            {
                return BadRequest("Invalid transfer request.");
            }

            try
            {
                var result = await _transactionService.InitiateTransferAsync(request);

                if (!result.Success)
                {
                    var errorResponse = new 
                    { 
                        success = false,
                        message = result.Message 
                    };

                    if (result.DebitOperation != null)
                    {
                        errorResponse = new
                        {
                            success = false,
                            message = result.Message,
                            //debitOperation = result.DebitOperation,
                            //creditOperation = result.CreditOperation
                        };
                    }

                    return BadRequest(errorResponse);
                }
               
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    transaction = result.Transaction,
                    debitOperation = new
                    {
                        accountNumber = result.DebitOperation.AccountNumber,
                        operationType = result.DebitOperation.OperationType,
                        amount = result.DebitOperation.Amount,
                        newBalance = result.DebitOperation.NewBalance,
                        status = result.DebitOperation.Status
                    },
                    creditOperation = new
                    {
                        accountNumber = result.CreditOperation.AccountNumber,
                        operationType = result.CreditOperation.OperationType,
                        amount = result.CreditOperation.Amount,
                        newBalance = result.CreditOperation.NewBalance,
                        status = result.CreditOperation.Status
                    }
                });
            }
            catch (Exception ex)
            {
                // Log exception (not shown)
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("{transactionId}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(Guid transactionId)
        {
            if (transactionId == Guid.Empty)
            {
                return BadRequest("Invalid transaction ID.");
            }

            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);

                if (transaction == null)
                {
                    return NotFound($"Transaction with ID {transactionId} not found.");
                }

             //  var transactionDto = new TransactionDto
              //  {
                //    Id = transaction.Id,
                 //   FromAccountId = transaction.FromAccountId,
                  //  ToAccountId = transaction.ToAccountId,
                   // Amount = transaction.Amount,
                  //  Currency = transaction.Currency,
                  //  Status = transaction.Status,
                   // Type = transaction.Type,
                   // Description = transaction.Description,
                   // Reference = transaction.Reference,
                  //  InitiatedAt = transaction.InitiatedAt,
                  //  CompletedAt = transaction.CompletedAt,
                   // FailureReason = transaction.FailureReason
               // }
               // ;

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                // Log exception (not shown)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        

    }


        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAccountTransactions(
    string accountId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return BadRequest("Account ID is required.");
            }

            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and pageSize must be positive integers.");
            }

            try
            {
                var transactions = await _transactionService.GetAccountTransactionsAsync(accountId, page, pageSize);

                if (transactions == null || !transactions.Any())
                {
                    return NotFound($"No transactions found for account {accountId}.");
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                // Log exception (not shown)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("account/{accountId}/filtered")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetFilteredAccountTransactions(
            string accountId,
            [FromQuery] string operation,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return BadRequest("Account ID is required.");
            }

            // Set default date range if not provided
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1); // Default: last month
            var end = endDate ?? DateTime.UtcNow; // Default: now

            if (start > end)
            {
                return BadRequest("Start date cannot be after end date.");
            }

            try
            {
                var transactions = await _transactionService.GetFilteredAccountTransactionAsync(
                    accountId, 
                    operation, 
                    start, 
                    end);

                if (transactions == null || !transactions.Any())
                {
                    return NotFound($"No transactions found for account {accountId} with the specified filters.");
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                // Log exception (not shown)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{transactionId}/cancel")]
        public async Task<ActionResult> CancelTransaction(Guid transactionId)
        {
            if (transactionId == Guid.Empty)
            {
                return BadRequest("Invalid transaction ID.");
            }

            try
            {
                var result = await _transactionService.CancelTransactionAsync(transactionId);

                if (!result)
                {
                    return NotFound($"Transaction with ID {transactionId} could not be cancelled or was not found.");
                }

                return Ok($"Transaction with ID {transactionId} has been cancelled.");
            }
            catch (Exception ex)
            {
                // Log exception (not shown)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}
