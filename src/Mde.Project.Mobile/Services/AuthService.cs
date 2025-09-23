using System.Net.Http.Json;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services
{
    public class AuthService : BaseApiService, IAuthService
    {
        public Task<JwtResponse?> LoginAsync(LoginModel login) =>
            ExecuteApiCallAsync<JwtResponse>(() =>
            {
                var url = new Uri(_httpClient.BaseAddress!, "api/auth/login");
                System.Diagnostics.Debug.WriteLine($"=== LOGIN DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
                System.Diagnostics.Debug.WriteLine($"Full URL: {url}");
                System.Diagnostics.Debug.WriteLine($"Username: {login.Username}");
                System.Diagnostics.Debug.WriteLine($"==================");
                return _httpClient.PostAsJsonAsync(url, login);
            }, withAuth: false);

        public Task<JwtResponse?> RegisterAsync(RegisterModel register) =>
            ExecuteApiCallAsync<JwtResponse>(() =>
            {
                var url = new Uri(_httpClient.BaseAddress!, "api/auth/register");
                System.Diagnostics.Debug.WriteLine($"=== REGISTER DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
                System.Diagnostics.Debug.WriteLine($"Full URL: {url}");
                System.Diagnostics.Debug.WriteLine($"Username: {register.Username}");
                System.Diagnostics.Debug.WriteLine($"Email: {register.Email}");
                System.Diagnostics.Debug.WriteLine($"====================");
                return _httpClient.PostAsJsonAsync(url, register);
            }, withAuth: false);

        public Task LogoutAsync()
        {
            SecureStorage.Remove(TokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return Task.CompletedTask;
        }
    }
}