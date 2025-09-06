using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services.Mock
{
    public class MockTrainingService : ITrainingService
    {
        private readonly List<TrainingEntryModel> _store = new();

        public Task<List<TrainingEntryModel>?> GetUserTrainingEntriesAsync() => Task.FromResult(_store.ToList());

        public Task<TrainingEntryModel?> CreateTrainingEntryAsync(TrainingEntryModel request)
        {
            _store.Add(request);
            return Task.FromResult(request);
        }
    }
}
