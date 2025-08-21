using Mde.Project.Mobile.Models;


namespace Mde.Project.Mobile.Interfaces
{
    public interface ITrainingService
    {
        Task<List<TrainingEntryModel>> GetTrainingsAsync(string jwtToken);
        Task<bool> CreateTrainingAsync(TrainingEntryModel model, string jwtToken);
        Task<bool> AddTrainingAsync(string jwtToken, TrainingEntryModel model);
    }
}
