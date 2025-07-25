using System.Net.Http.Json;
using Mde.Project.Mobile.Models;
using System.Text;
using System.Text.Json;

namespace Mde.Project.Mobile.Services;

public class TrainingService
{
    private readonly HttpClient _httpClient;

    public TrainingService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:62160/") // Pas aan indien nodig
        };
    }

    public async Task<List<TrainingEntryModel>> GetTrainingsAsync(string jwtToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var result = await _httpClient.GetFromJsonAsync<List<TrainingEntryModel>>("api/trainingentries/by-user");

            return result ?? new List<TrainingEntryModel>();
        }
        catch
        {
            return new List<TrainingEntryModel>();
        }
    }

    public async Task<bool> CreateTrainingAsync(TrainingEntryModel model, string jwtToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/trainingentries", content);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
