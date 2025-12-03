using Microsoft.Extensions.Logging;
using System.Net;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TransactionMicroservices.Model;

namespace TransactionMicroservices.Clients
{
    public class GatewayClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GatewayClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public GatewayClient(IHttpClientFactory httpClientFactory, ILogger<GatewayClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient GetAuthenticatedHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient("ApiGateway");
            
            // Get the token from the current request's Authorization header
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                // Forward the bearer token to the gateway
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);
                }
            }
            
            return httpClient;
        }
        
        public async Task<bool> ValidateAccountExistsAsync(string accountNumber)
        {
            _logger.LogInformation("Validating account: {AccountNumber}", accountNumber);
            
            var httpClient = GetAuthenticatedHttpClient();
            var response = await httpClient.GetAsync($"/account/{accountNumber}");
            
            _logger.LogInformation("Account validation response: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
                return true;

            // If account not found -> API returns 404
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            // For any other error -> log and return false
            _logger.LogWarning("Account validation failed with status: {StatusCode}", response.StatusCode);
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

            var httpClient = GetAuthenticatedHttpClient();
            var response = await httpClient.PutAsync($"/account/update-status/{accountNumber}", content);
            
            _logger.LogInformation("Lock account response: {StatusCode}", response.StatusCode);
            
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> CheckBalanceAsync(decimal Amount, string accountNumber)
        {
            var httpClient = GetAuthenticatedHttpClient();
            var response = await httpClient.GetAsync($"/account/get-account-balance/{accountNumber}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Check balance failed: {StatusCode}", response.StatusCode);
                return false;
            }

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

            var httpClient = GetAuthenticatedHttpClient();
            var response = await httpClient.PutAsync($"account/update-balance/{accountNumber}", content);
            
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

            var httpClient = GetAuthenticatedHttpClient();
            var response = await httpClient.PutAsync($"/account/update-status/{accountNumber}", content);
            
            return response.IsSuccessStatusCode;
        }
    }
}