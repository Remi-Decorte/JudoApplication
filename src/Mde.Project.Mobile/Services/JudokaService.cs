using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services;

public class JudokasService : BaseApiService, IJudokaService
{
    public JudokasService() : base() { }

    public async Task<List<string>> GetCategoriesAsync()
    {
        // This would need to be implemented in your API
        // For now, return common judo categories
        await Task.CompletedTask;
        return new List<string> { "-60", "-66", "-73", "-81", "-90", "-100", "+100" };
    }

    public async Task<List<JudokaModel>?> GetJudokasByCategoryAsync(string category)
    {
        return await ExecuteApiCallAsync<List<JudokaModel>>(() =>
            _httpClient.GetAsync($"judokas/by-category/{category}"));
    }
}
