using Azure.Core;
using TransactionMicroservices.Model.DTO;
namespace TransactionMicroservices.Services.Helper
{
    public class Helper
    {
        bool ValidateAccounts(TransactionRequest request)
        {
            return true;

        }


        bool LockAccount(TransactionRequest request)
        {
            return true;
        }
       
        bool CheckBalance(TransactionRequest request)
        {
          return true;
        }

        bool DebitAccount(TransactionRequest request)
        {
            return true;
        }

        
       // await CreditAccount(TransactionRequest request);

    }
}
