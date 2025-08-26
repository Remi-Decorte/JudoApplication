using Mde.Project.Mobile.Models;


namespace Mde.Project.Mobile.Interfaces
{
    public interface ITrainingService
    {
        Task<List<TrainingEntryModel>?> GetUserTrainingEntriesAsync();
        Task<TrainingEntryModel?> CreateTrainingEntryAsync(TrainingEntryModel request);
    }
}
