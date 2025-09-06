using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;
using System.Linq;

namespace Mde.Project.Mobile.ViewModels
{
    public class AthletesViewModel : INotifyPropertyChanged
    {
        private readonly IJudokaService _judokaService;

        public AthletesViewModel(IJudokaService judokaService)
        {
            _judokaService = judokaService;
        }

        // Publieke collecties gebonden in XAML
        public ObservableCollection<string> Categories { get; } = new();
        public ObservableCollection<JudokaModel> Judokas { get; } = new();

        private string _selectedCategory = string.Empty;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory == value) return;
                _selectedCategory = value;
                OnPropertyChanged();
                _ = LoadJudokasAsync();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        // ?? Eén publieke entrypoint voor de pagina
        public async Task LoadAsync()
        {
            // Eerst categorieën (zet SelectedCategory) en daarna lijst
            await LoadCategoriesAsync();
            await LoadJudokasAsync();
        }

        public async Task LoadCategoriesAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Categories.Clear();

                var cats = await _judokaService.GetCategoriesAsync();
                if (cats != null && cats.Count > 0)
                {
                    foreach (var c in cats) Categories.Add(c);

                    // Zorg dat SelectedCategory geldig is
                    if (string.IsNullOrWhiteSpace(SelectedCategory) || !cats.Contains(SelectedCategory))
                        SelectedCategory = cats.First();
                }
                else
                {
                    // fallback
                    var defaults = new[] { "-60", "-66", "-73", "-81", "-90", "-100", "+100" };
                    foreach (var c in defaults) Categories.Add(c);
                    if (string.IsNullOrWhiteSpace(SelectedCategory) || !defaults.Contains(SelectedCategory))
                        SelectedCategory = defaults.First();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadJudokasAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(SelectedCategory)) return;
            IsBusy = true;

            try
            {
                Judokas.Clear();
                var list = await _judokaService.GetJudokasByCategoryAsync(SelectedCategory);
                if (list != null)
                {
                    foreach (var j in list) Judokas.Add(j);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
