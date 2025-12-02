using System.Reflection.Metadata.Ecma335;
using TransactionMicroservices.Model;
using TransactionMicroservices.Model.DTO;
namespace TransactionMicroservices.IServiceContracts
{
    public interface ITransactionService
    {
        public Task<TransactionResult> InitiateTransferAsync(TransactionRequest request);

        public Task<TransactionDto> GetTransactionByIdAsync(Guid transactionId);
        public Task<IEnumerable<TransactionDto>> GetAccountTransactionsAsync(string accountId, int page, int pageSize);
        public Task<IEnumerable<TransactionDto>> GetFilteredAccountTransactionAsync(string accountId, string operation, DateTime start, DateTime end);    
        public Task<bool> CancelTransactionAsync(Guid transactionId);
    }
}
