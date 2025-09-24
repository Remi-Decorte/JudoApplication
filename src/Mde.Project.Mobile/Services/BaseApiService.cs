using System.Net;
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

            System.Diagnostics.Debug.WriteLine("=== HTTP CLIENT INIT ===");
            System.Diagnostics.Debug.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
            System.Diagnostics.Debug.WriteLine($"Timeout: {_httpClient.Timeout}");
            System.Diagnostics.Debug.WriteLine("========================");
        }

        protected async Task SetAuthorizationHeaderAsync()
        {
            var token = await SecureStorage.GetAsync(TokenKey);
            _httpClient.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Voor calls met JSON-response (GET/POST/PUT die iets teruggeven).
        /// Retourneert default(T) bij 204/lege body.
        /// </summary>
        protected async Task<T?> ExecuteApiCallAsync<T>(
            Func<Task<HttpResponseMessage>> apiCall,
            bool withAuth = true)
        {
            try
            {
                if (withAuth) await SetAuthorizationHeaderAsync();
                else _httpClient.DefaultRequestHeaders.Authorization = null;

                System.Diagnostics.Debug.WriteLine("=== API CALL START ===");
                var resp = await apiCall();

                System.Diagnostics.Debug.WriteLine($"Status:  {resp.StatusCode} ({(int)resp.StatusCode})");
                System.Diagnostics.Debug.WriteLine($"Method:  {resp.RequestMessage?.Method}");
                System.Diagnostics.Debug.WriteLine($"URL:     {resp.RequestMessage?.RequestUri}");

                if (!resp.IsSuccessStatusCode)
                {
                    var bodyText = await resp.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error Body: {bodyText}");
                    throw new Exception($"API call failed: {(int)resp.StatusCode} {resp.ReasonPhrase} - {bodyText}");
                }

                // 204 No Content of lege body → return default
                if (resp.StatusCode == HttpStatusCode.NoContent ||
                    resp.Content == null ||
                    (resp.Content.Headers.ContentLength ?? 0) == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No content returned.");
                    return default;
                }

                var result = await resp.Content.ReadFromJsonAsync<T>();
                System.Diagnostics.Debug.WriteLine($"Success! Parsed as {typeof(T).Name}");
                System.Diagnostics.Debug.WriteLine("=== API CALL END ===");
                return result;
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request Exception: {httpEx.Message}");
                throw new Exception($"Network error: {httpEx.Message}", httpEx);
            }
            catch (TaskCanceledException tcEx)
            {
                System.Diagnostics.Debug.WriteLine($"Request Timeout: {tcEx.Message}");
                throw new Exception("Request timeout. Check your connection/API.", tcEx);
            }
        }

        /// <summary>
        /// Voor calls waar je GEEN body verwacht (bv. DELETE).
        /// </summary>
        protected async Task ExecuteNoContentAsync(
            Func<Task<HttpResponseMessage>> apiCall,
            bool withAuth = true)
        {
            if (withAuth) await SetAuthorizationHeaderAsync();
            else _httpClient.DefaultRequestHeaders.Authorization = null;

            var resp = await apiCall();
            if (!resp.IsSuccessStatusCode)
            {
                var bodyText = await resp.Content.ReadAsStringAsync();
                throw new Exception($"API call failed: {(int)resp.StatusCode} {resp.ReasonPhrase} - {bodyText}");
            }
            // niets te deserializen
        }
    }
}
