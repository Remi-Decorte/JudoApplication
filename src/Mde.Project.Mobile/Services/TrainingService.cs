using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.Services.Interfaces;

namespace Mde.Project.Mobile.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly HttpClient _httpClient;

        public TrainingService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:62160/") 
            };
        }

        public async Task<List<TrainingEntryModel>> GetTrainingsAsync(string jwtToken)
        {
            // normale API call voor als we geen MOCK data gebruiken maar api
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

            // MOCK DATA
            await Task.Delay(500); // Simuleer wachttijd
            return new List<TrainingEntryModel>
            {
                new TrainingEntryModel
                {
                    Date = DateTime.Today,
                    Type = "Randori training",
                    TechniqueScores = new List<TechniqueScoreModel>
                    {
                        new TechniqueScoreModel { Technique = "Uchi mata", ScoreCount = 3 },
                        new TechniqueScoreModel { Technique = "Seoi nage", ScoreCount = 2 }
                    }
                },
                new TrainingEntryModel
                {
                    Date = DateTime.Today.AddDays(-1),
                    Type = "Kracht training",
                    TechniqueScores = new List<TechniqueScoreModel>()
                }
            };
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

            await Task.Delay(300);
            return true; // Altijd succes bij mock
        }
    }
}
