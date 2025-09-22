using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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
                BaseAddress = new Uri("https://65n69d92-62160.euw.devtunnels.ms/api/")
            };
        }

        protected async Task SetAuthorizationHeaderAsync()
        {
            var token = await SecureStorage.GetAsync(TokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected async Task<T?> ExecuteApiCallAsync<T>(Func<Task<HttpResponseMessage>> apiCall)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var response = await apiCall();
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (HttpRequestException ex)
            {
                // Handle unauthorized or other HTTP errors
                throw new Exception($"API call failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }
    }
}