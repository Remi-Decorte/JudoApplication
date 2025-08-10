using System.Text;
using System.Text.Json;
// using System.Net.Http.Json; // <- is voor API
using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.Services.Interfaces;

namespace Mde.Project.Mobile.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("https://localhost:62160/")
        };

        public async Task<List<TrainingEntryModel>> GetTrainingsAsync(string jwtToken)
        {
            //-API in comment gezet als ik api wil gebruiken dit uit comment halen
            /*
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

                var result = await _httpClient.GetFromJsonAsync<List<TrainingEntryModel>>("api/trainingentries/by-user");
                return result ?? new List<TrainingEntryModel>();
            }
            catch
            {
                return new List<TrainingEntryModel>();
            }
            */
            return new();
        }

        public async Task<bool> CreateTrainingAsync(TrainingEntryModel model, string jwtToken)
        {
            /*
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/trainingentries", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
            */
            return false; // placeholder voor MOCK
        }
    }
}
