using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;


namespace Mde.Project.Mobile.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public TrainingService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:62160/") 
            };
        }

        public async Task<List<TrainingEntryModel>> GetTrainingsAsync(string jwtToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

                var result = await _httpClient
                    .GetFromJsonAsync<List<TrainingEntryModel>>("api/trainingentries/by-user", _jsonOptions);

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
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

                var json = JsonSerializer.Serialize(model, _jsonOptions);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/trainingentries", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public Task<bool> AddTrainingAsync(string jwtToken, TrainingEntryModel model)
            => CreateTrainingAsync(model, jwtToken);

    }
}
