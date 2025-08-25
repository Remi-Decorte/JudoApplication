using System.Net.Http.Json;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services;

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
            {
                var result = await response.Content.ReadFromJsonAsync<JwtResponse>();
                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            return null;

        }
    }

    public Task LogoutAsync()
    {
        throw new NotImplementedException();
    }
}


