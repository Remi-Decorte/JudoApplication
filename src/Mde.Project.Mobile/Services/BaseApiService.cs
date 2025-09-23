using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Mde.Project.Mobile.Services
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected const string TokenKey = "jwt_token";

        protected BaseApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://7lvw5x4s-62160.euw.devtunnels.ms/"),
                Timeout = TimeSpan.FromSeconds(30) // Verhoog timeout
            };

            System.Diagnostics.Debug.WriteLine($"=== HTTP CLIENT INIT ===");
            System.Diagnostics.Debug.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
            System.Diagnostics.Debug.WriteLine($"Timeout: {_httpClient.Timeout}");
            System.Diagnostics.Debug.WriteLine($"========================");
        }

        protected async Task SetAuthorizationHeaderAsync()
        {
            var token = await SecureStorage.GetAsync(TokenKey);
            _httpClient.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
        }

        protected async Task<T?> ExecuteApiCallAsync<T>(
            Func<Task<HttpResponseMessage>> apiCall,
            bool withAuth = true)
        {
            try
            {
                if (withAuth) await SetAuthorizationHeaderAsync();
                else _httpClient.DefaultRequestHeaders.Authorization = null;

                System.Diagnostics.Debug.WriteLine($"=== API CALL START ===");

                var resp = await apiCall();

                System.Diagnostics.Debug.WriteLine($"Response Status: {resp.StatusCode} ({(int)resp.StatusCode})");
                System.Diagnostics.Debug.WriteLine($"Request URL: {resp.RequestMessage?.RequestUri}");
                System.Diagnostics.Debug.WriteLine($"Request Method: {resp.RequestMessage?.Method}");

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error Response Body: {body}");
                    System.Diagnostics.Debug.WriteLine($"Response Headers: {resp.Headers}");
                    System.Diagnostics.Debug.WriteLine($"Content Headers: {resp.Content.Headers}");

                    throw new Exception($"API call failed: {(int)resp.StatusCode} {resp.ReasonPhrase} - {body}");
                }

                var result = await resp.Content.ReadFromJsonAsync<T>();
                System.Diagnostics.Debug.WriteLine($"Success! Result type: {typeof(T).Name}");
                System.Diagnostics.Debug.WriteLine($"=== API CALL END ===");

                return result;
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request Exception: {httpEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {httpEx.InnerException?.Message}");
                throw new Exception($"Network error: {httpEx.Message}", httpEx);
            }
            catch (TaskCanceledException tcEx)
            {
                System.Diagnostics.Debug.WriteLine($"Request Timeout: {tcEx.Message}");
                throw new Exception("Request timeout. Check your internet connection and API availability.", tcEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}