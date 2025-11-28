
using System.Reflection.Metadata.Ecma335;
using TransactionMicroservices.Model.DTO;
namespace TransactionMicroservices.IServiceContracts
{
    public interface ITransactionService
    {
        public Task<TransactionDto> InitiateTransferAsync(TransactionRequest request);

        public Task<TransactionDto> GetTransactionByIdAsync(Guid transactionId);
        Task<IEnumerable<TransactionDto>> GetAccountTransactionsAsync(string accountId, int page, int pageSize);

        public Task<bool> CancelTransactionAsync(Guid transactionId);
    }
}
