using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services
{
    public class JudokasService : BaseApiService, IJudokaService
    {
        public JudokasService() : base() { }

        public async Task<List<string>> GetCategoriesAsync()
        {
            await Task.CompletedTask;
            return new List<string> { "-60", "-66", "-73", "-81", "-90", "-100", "+100" };
        }

        public Task<List<JudokaModel>?> GetJudokasByCategoryAsync(string category) =>
            ExecuteApiCallAsync<List<JudokaModel>>(() =>
            {
                var url = new Uri(_httpClient.BaseAddress!,
                    $"api/Judokas/by-category/{Uri.EscapeDataString(category)}");
                System.Diagnostics.Debug.WriteLine("GET " + url);
                return _httpClient.GetAsync(url);
            });
    }
}
