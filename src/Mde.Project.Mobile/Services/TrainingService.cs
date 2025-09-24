using System.Net.Http.Json;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services
{
    public class TrainingService : BaseApiService, ITrainingService
    {
        public Task<List<TrainingEntryModel>?> GetUserTrainingEntriesAsync() =>
            ExecuteApiCallAsync<List<TrainingEntryModel>>(
                () => _httpClient.GetAsync("api/trainingentries/by-user"));

        public Task<TrainingEntryModel?> CreateTrainingEntryAsync(TrainingEntryModel request) =>
            ExecuteApiCallAsync<TrainingEntryModel>(
                () => _httpClient.PostAsJsonAsync("api/trainingentries", request));

        public Task<TrainingEntryModel?> UpdateTrainingEntryAsync(TrainingEntryModel request) =>
            ExecuteApiCallAsync<TrainingEntryModel>(
                () => _httpClient.PutAsJsonAsync($"api/trainingentries/{request.Id}", request));

        //NoContent-variant gebruiken
        public Task DeleteTrainingEntryAsync(int id) =>
            ExecuteNoContentAsync(() => _httpClient.DeleteAsync($"api/trainingentries/{id}"));
    }
}
