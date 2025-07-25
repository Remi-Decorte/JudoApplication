using System.Net.Http.Json;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services;

public class JudokaService
{
    private readonly HttpClient _httpClient;

    public JudokaService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:62160/") 
        };
    }

    public async Task<List<JudokaModel>> GetJudokasByCategoryAsync(string category)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<JudokaModel>>($"api/judokas/by-category/{category}");
            return response ?? new List<JudokaModel>();
        }
        catch
        {
            return new List<JudokaModel>(); 
        }
    }
}
