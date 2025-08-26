using Mde.Project.Mobile.Models;


namespace Mde.Project.Mobile.Interfaces
{
    public interface IJudokaService
    {
        Task<List<string>> GetCategoriesAsync();
        Task<List<JudokaModel>?> GetJudokasByCategoryAsync(string category);
    }
}
