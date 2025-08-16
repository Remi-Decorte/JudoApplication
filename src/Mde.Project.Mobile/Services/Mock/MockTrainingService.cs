using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services.Mock
{
    public class MockTrainingService : ITrainingService
    {
        private readonly List<TrainingEntryModel> _store = new();

        public Task<List<TrainingEntryModel>> GetTrainingsAsync(string jwtToken)
            => Task.FromResult(_store.ToList());

        public Task<bool> CreateTrainingAsync(TrainingEntryModel model, string jwtToken)
        {
            _store.Add(model);
            return Task.FromResult(true);
        }
    }
}
