using Mde.Project.Mobile.Models;


namespace Mde.Project.Mobile.Interfaces
{
    public interface ITrainingService
    {
        Task<List<TrainingEntryModel>> GetTrainingsAsync(string jwtToken);
        Task<bool> CreateTrainingAsync(TrainingEntryModel model, string jwtToken);
        Task AddTrainingAsync(TrainingModel training, string jwtToken);
    }
}
