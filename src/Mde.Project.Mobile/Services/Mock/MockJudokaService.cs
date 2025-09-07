using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services.Mock
{
    public class MockJudokaService : IJudokaService
    {
        private readonly Dictionary<string, List<JudokaModel>> _data = new();

        public MockJudokaService()
        {
            _data["-60"] = new List<JudokaModel>
            {
                new JudokaModel { FullName = "Judoka A", Country = "BEL" },
                new JudokaModel { FullName = "Judoka B", Country = "FRA" },
                new JudokaModel { FullName = "Judoka C", Country = "NED" },
                new JudokaModel { FullName = "Judoka D", Country = "GER" },
                new JudokaModel { FullName = "Judoka E", Country = "JPN" },
                new JudokaModel { FullName = "Judoka F", Country = "DE" }
            };

            _data["-66"] = new List<JudokaModel>
            {
                new JudokaModel { FullName = "Judoka F", Country = "BRA" },
                new JudokaModel { FullName = "Judoka G", Country = "USA" },
                new JudokaModel { FullName = "Judoka H", Country = "ESP" },
                new JudokaModel { FullName = "Judoka I", Country = "ITA" },
                new JudokaModel { FullName = "Judoka J", Country = "KOR" }
            };
        }

        public Task<List<string>> GetCategoriesAsync()
        {
            // retourneert gewoon de keys van de dictionary
            return Task.FromResult(_data.Keys.ToList());
        }

        public Task<List<JudokaModel>> GetJudokasByCategoryAsync(string category)
        {
            if (_data.TryGetValue(category, out var list))
                return Task.FromResult(list);

            return Task.FromResult(new List<JudokaModel>());
        }
    }
}
