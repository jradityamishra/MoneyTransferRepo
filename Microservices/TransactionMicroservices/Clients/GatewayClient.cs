using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Net;
using System.Text;
using System.Text.Json;
using TransactionMicroservices.Model;

namespace TransactionMicroservices.Clients
{
    public class GatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GatewayClient> _logger;
        
        public GatewayClient(IHttpClientFactory httpClient, ILogger<GatewayClient> logger)
        {
            _httpClient = httpClient.CreateClient("ApiGateway");
            _logger = logger;
        }
        
        public async Task<bool> ValidateAccountExistsAsync(string accountNumber)
        { 
            var response = await _httpClient.GetAsync($"/account/{accountNumber}");
           
            if (response.IsSuccessStatusCode)
                return true;

            // If account not found -> API returns 404
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            // For any other error -> throw or handle
            return false;
        }

        public async Task<bool> LockAccountAsync(string accountNumber, string statusType)
        {
            var requestBody = new 
            { 
                AccountNumber = accountNumber,
                Status = statusType 
            };
            
            _logger.LogInformation("Lock account request: {Request}", JsonSerializer.Serialize(requestBody));
            
            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync($"/account/update-status/{accountNumber}", content);
            _logger.LogInformation("Lock account response: {Response}", response.StatusCode);
            
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> CheckBalanceAsync(decimal Amount, string accountNumber)
        {
            var response = await _httpClient.GetAsync($"/account/get-account-balance/{accountNumber}");
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadAsStringAsync();
            var currentBalance = decimal.Parse(result);
            return currentBalance >= Amount;
        }
 

        public async Task<BalanceUpdateResponse> UpdateAccountBalanceAsync(string accountNumber, decimal amount, string operation)
        {
            var request = new BalanceUpdateRequest
            {
                AccountNumber = accountNumber,
                Amount = (int)amount,
                operation = operation,
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync($"account/update-balance/{accountNumber}", content);
            
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BalanceUpdateResponse>(resultContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                return new BalanceUpdateResponse
                {
                    Success = true,
                    Message = result?.Message ?? "Balance updated successfully",
                    Balance = result?.Balance
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorResult = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    var message = errorResult.TryGetProperty("message", out var msgProp) 
                        ? msgProp.GetString() 
                        : "Failed to update balance";
                    
                    return new BalanceUpdateResponse
                    {
                        Success = false,
                        Message = message
                    };
                }
                catch
                {
                    return new BalanceUpdateResponse
                    {
                        Success = false,
                        Message = "Failed to update balance"
                    };
                }
            }
        }

        public async Task<bool> UnlockAccountAsync(string accountNumber, string statusType)
        {
            var requestBody = new 
            { 
                AccountNumber = accountNumber,
                Status = statusType 
            };
            
            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync($"/account/update-status/{accountNumber}", content);
            return response.IsSuccessStatusCode;
        }
    }
}