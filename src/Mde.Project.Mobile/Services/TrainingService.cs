using System.Net.Http.Json;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;


namespace Mde.Project.Mobile.Services
{
    public class TrainingService : BaseApiService, ITrainingService
    {
        public async Task<List<TrainingEntryModel>?> GetUserTrainingEntriesAsync()
        {
            return await ExecuteApiCallAsync<List<TrainingEntryModel>>(() =>
                _httpClient.GetAsync("trainingentries/by-user"));
        }

        public async Task<TrainingEntryModel?> CreateTrainingEntryAsync(TrainingEntryModel request)
        {
            return await ExecuteApiCallAsync<TrainingEntryModel>(() =>
                _httpClient.PostAsJsonAsync("trainingentries", request));
        }
    }
}
