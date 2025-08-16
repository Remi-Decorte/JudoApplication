using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services.Mock
{
    public class MockJudokaService : IJudokaService
    {
        public Task<List<JudokaModel>> GetJudokasByCategoryAsync(string category)
        {
            // Demo data – categorie wordt niet strikt gefilterd in mock
            return Task.FromResult(new List<JudokaModel>
            {
                new() { FullName = "Hifumi Abe", Country = "Japan",  Category = category, Ranking = 1 },
                new() { FullName = "Lasha Shavdatuashvili", Country = "Georgia", Category = category, Ranking = 2 },
                new() { FullName = "Luka Mkheidze", Country = "France", Category = category, Ranking = 3 },
                new() { FullName = "Vazha Margvelashvili", Country = "Georgia", Category = category, Ranking = 4 },
                new() { FullName = "Fabio Basile", Country = "Italy", Category = category, Ranking = 5 },
            });
        }
    }
}
