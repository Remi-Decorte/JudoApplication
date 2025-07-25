using System.Net.Http.Json;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:62160/") 
        };
    }

    public async Task<string?> LoginAsync(LoginModel login)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", login);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JwtResponse>();
                return result?.Token;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}


