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
            // ROOT van je tunnel (geen /swagger, geen /api)
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://7lvw5x4s-62160.euw.devtunnels.ms/")
            };
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
            if (withAuth) await SetAuthorizationHeaderAsync();
            else _httpClient.DefaultRequestHeaders.Authorization = null;

            var resp = await apiCall();
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();   //toon serverboodschap
                throw new Exception($"API call failed: {(int)resp.StatusCode} {resp.ReasonPhrase} - {body}");
            }
            return await resp.Content.ReadFromJsonAsync<T>();
        }
    }
}
