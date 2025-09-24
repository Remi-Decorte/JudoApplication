using System;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Collections.Generic;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services
{
    public class TrainingService : BaseApiService, ITrainingService
    {
        public Task<List<TrainingEntryModel>?> GetUserTrainingEntriesAsync() =>
            ExecuteApiCallAsync<List<TrainingEntryModel>>(() =>
            {
                var url = new Uri(_httpClient.BaseAddress!, "api/TrainingEntries/by-user");
                System.Diagnostics.Debug.WriteLine("GET " + url);
                return _httpClient.GetAsync(url);
            });

        public Task<TrainingEntryModel?> CreateTrainingEntryAsync(TrainingEntryModel request) =>
            ExecuteApiCallAsync<TrainingEntryModel>(() =>
            {
                var url = new Uri(_httpClient.BaseAddress!, "api/TrainingEntries");
                System.Diagnostics.Debug.WriteLine("POST " + url);
                return _httpClient.PostAsJsonAsync(url, request);
            });
        public Task<TrainingEntryModel?> UpdateTrainingEntryAsync(TrainingEntryModel request) =>
       ExecuteApiCallAsync<TrainingEntryModel>(() => _httpClient.PutAsJsonAsync($"api/trainingentries/{request.Id}", request));

        public Task DeleteTrainingEntryAsync(int id) =>
            ExecuteApiCallAsync<object?>(() => _httpClient.DeleteAsync($"api/trainingentries/{id}"));
    }
}
