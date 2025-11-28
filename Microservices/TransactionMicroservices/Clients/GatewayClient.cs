using System.Text;
using System.Text.Json;

namespace TransactionMicroservices.Clients
{
    public class GatewayClient
    {
        private readonly HttpClient _httpClient;
        public GatewayClient(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("ApiGateway");
        }
        public async Task<bool> ValidateAccountExistsAsync(string accountID)
        { 
          var response = await _httpClient.GetAsync($"/api/accounts/{accountID}/exists");
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadAsStringAsync();
            return bool.Parse(result);
        }

        public async Task<bool> LockAccountAsync(string accountID)
        {
            var response = await _httpClient.PostAsync($"/api/accounts/{accountID}/lock", null);
            return response.IsSuccessStatusCode;
        }
        

        public async Task<bool> CheckBalanceAsync(decimal Amount, string accountID)
        {
            var response = await _httpClient.GetAsync($"/api/accounts/{accountID}/balance");
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadAsStringAsync();
            var currentBalance = decimal.Parse(result);
            return currentBalance >= Amount;
        }
 

        public async Task<bool> UpdateAccountBalanceAsync(string accountID, decimal amount, string operation)
        {
            var request = new BalanceUpdateRequest
            {
                Amount = amount,
                Operation = operation
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync($"/api/accounts/{accountID}/balance", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UnlockAccountAsync(string accountID)
        {
            var response = await _httpClient.PutAsync($"/api/accounts/{accountID}/unlock", null);
            return response.IsSuccessStatusCode;
        }

    }
}
