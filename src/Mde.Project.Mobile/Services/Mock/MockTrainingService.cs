using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services
{
    public class MockTrainingService : ITrainingService
    {
        private readonly List<TrainingEntryModel> _store = new();
        private int _nextId = 1;

        public Task<List<TrainingEntryModel>?> GetUserTrainingEntriesAsync() =>
            Task.FromResult(_store.ToList());

        public Task<TrainingEntryModel?> CreateTrainingEntryAsync(TrainingEntryModel request)
        {
            if (request.Id == 0) request.Id = _nextId++;
            _store.Add(request);
            return Task.FromResult<TrainingEntryModel?>(request);
        }

        public Task<TrainingEntryModel?> UpdateTrainingEntryAsync(TrainingEntryModel request)
        {
            var idx = _store.FindIndex(t => t.Id == request.Id);
            if (idx >= 0) _store[idx] = request;
            return Task.FromResult<TrainingEntryModel?>(request);
        }

        public Task DeleteTrainingEntryAsync(int id)
        {
            _store.RemoveAll(t => t.Id == id);
            return Task.CompletedTask;
        }
    }
}
