using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.ViewModels
{
    public class AthletesViewModel : INotifyPropertyChanged
    {
        private readonly IJudokaService _judokaService;

        public AthletesViewModel(IJudokaService judokaService)
        {
            _judokaService = judokaService;
            _ = LoadCategoriesAsync();
        }

        public ObservableCollection<string> Categories { get; } = new();
        public ObservableCollection<JudokaModel> Judokas { get; } = new();

        private string _selectedCategory = "-60kg";
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

        public async Task LoadCategoriesAsync()
        {
            Categories.Clear();

            var cats = await _judokaService.GetCategoriesAsync();
            if (cats?.Count > 0)
            {
                foreach (var c in cats) Categories.Add(c);
                if (!cats.Contains(SelectedCategory))
                    SelectedCategory = cats.First();
            }
            else
            {
                // hardcoded
                var defaults = new[] { "-60kg", "-66kg", "-73kg", "-81kg", "-90kg" };
                foreach (var c in defaults) Categories.Add(c);
                if (!defaults.Contains(SelectedCategory))
                    SelectedCategory = defaults.First();
            }

            await LoadJudokasAsync();
        }

        public async Task LoadJudokasAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(SelectedCategory)) return;
            IsBusy = true;

            try
            {
                Judokas.Clear();
                var list = await _judokaService.GetJudokasByCategoryAsync(SelectedCategory);
                foreach (var j in list) Judokas.Add(j);
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


