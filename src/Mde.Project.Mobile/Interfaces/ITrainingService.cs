using Mde.Project.Mobile.Models;


namespace Mde.Project.Mobile.Interfaces
{
    public interface ITrainingService
    {
        Task<List<TrainingEntryModel>?> GetUserTrainingEntriesAsync();
        Task<TrainingEntryModel?> CreateTrainingEntryAsync(TrainingEntryModel request);
        Task<TrainingEntryModel?> UpdateTrainingEntryAsync(TrainingEntryModel request);
        Task DeleteTrainingEntryAsync(int id);
    }
}
