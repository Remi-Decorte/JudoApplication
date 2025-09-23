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
                System.Diagnostics.Debug.WriteLine("POST " + url);
                return _httpClient.PostAsJsonAsync(url, login);
            }, withAuth: false);

        public Task<JwtResponse?> RegisterAsync(RegisterModel register) =>
            ExecuteApiCallAsync<JwtResponse>(() =>
            {
                var url = new Uri(_httpClient.BaseAddress!, "api/auth/register");
                System.Diagnostics.Debug.WriteLine("POST " + url);
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
