using System.Net.Http.Json;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services
{
    public class AuthService : BaseApiService, IAuthService
    {
        public async Task<JwtResponse?> LoginAsync(LoginModel login)
        {
            return await ExecuteApiCallAsync<JwtResponse>(() =>
                _httpClient.PostAsJsonAsync("auth/login", login));
        }

        public async Task<JwtResponse?> RegisterAsync(RegisterModel register)
        {
            return await ExecuteApiCallAsync<JwtResponse>(() =>
                _httpClient.PostAsJsonAsync("auth/register", register));
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Remove(TokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
