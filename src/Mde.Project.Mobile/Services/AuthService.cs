using System.Net.Http.Json;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://fb96g0tc-62160.uks1.devtunnels.ms/api/")
            };
        }

        public async Task<JwtResponse?> LoginAsync(LoginModel login)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", login);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<JwtResponse>();
                return null;
            }
            catch { return null; }
        }

        public async Task<JwtResponse?> RegisterAsync(RegisterModel register)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/register", register);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<JwtResponse>();
                return null;
            }
            catch { return null; }
        }

        public Task LogoutAsync() => Task.CompletedTask;
    }
}
