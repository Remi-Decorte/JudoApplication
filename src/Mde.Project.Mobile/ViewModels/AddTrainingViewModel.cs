using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.ViewModels
{
    public class AddTrainingViewModel : INotifyPropertyChanged
    {
        private const string LocalFileName = "trainings_local.json";
        private readonly IJudokaService _judokaService;
        // (No _trainingService here yet, as backend integration comes later)

        public AddTrainingViewModel(IJudokaService judokaService)
        {
            _judokaService = judokaService;

            Date = DateTime.Today;
            TrainingTypes = new() { "Techniek", "Conditioneel", "Wedstrijdvoorbereiding", "Herstel", "Randori" };
            SelectedType = TrainingTypes[0];  // default to first option ("Techniek")

            // Techniques list will be set in SelectedType property (Default is not Randori, so empty techniques)
            Techniques = new ObservableCollection<TechniqueScoreModel>();

            OpponentNotes = new ObservableCollection<OpponentNoteModel>();
            Judokas = new ObservableCollection<JudokaModel>();
            Categories = new ObservableCollection<string>();

            SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
            IncrementCommand = new Command<TechniqueScoreModel>(t =>
            {
                if (t == null) return;
                t.ScoreCount++;
                RefreshTechniques();
            });
            DecrementCommand = new Command<TechniqueScoreModel>(t =>
            {
                if (t == null) return;
                if (t.ScoreCount > 0) t.ScoreCount--;
                RefreshTechniques();
            });
            AddOpponentCommand = new Command(AddOpponentNote, CanAddOpponentNote);
            RemoveOpponentCommand = new Command<OpponentNoteModel>(RemoveOpponentNote);

            _ = InitCategoriesAsync();
        }

        // =========== Properties ===========
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        public List<string> TrainingTypes { get; }

        private string _selectedType = string.Empty;
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType == value) return;
                _selectedType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRandori));
                UpdateTechniques();
            }
        }

        public bool IsRandori =>
            SelectedType?.Equals("randori", StringComparison.OrdinalIgnoreCase) == true;

        private ObservableCollection<TechniqueScoreModel> _techniques = new();
        public ObservableCollection<TechniqueScoreModel> Techniques
        {
            get => _techniques;
            set { _techniques = value; OnPropertyChanged(); }
        }

        private bool _isBusyFlag;
        public bool IsBusy
        {
            get => _isBusyFlag;
            set
            {
                _isBusyFlag = value;
                OnPropertyChanged();
                (SaveCommand as Command)?.ChangeCanExecute();
            }
        }

        // ====== Judokas & categorieën ======
        public ObservableCollection<string> Categories { get; }
        private string? _selectedCategory;
        public string? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory == value) return;
                _selectedCategory = value;
                OnPropertyChanged();
                _ = LoadJudokasByCategoryAsync();
            }
        }

        public ObservableCollection<JudokaModel> Judokas { get; }
        private JudokaModel? _selectedJudoka;
        public JudokaModel? SelectedJudoka
        {
            get => _selectedJudoka;
            set
            {
                _selectedJudoka = value;
                OnPropertyChanged();
                ((Command)AddOpponentCommand).ChangeCanExecute();
            }
        }

        private string _opponentComment = string.Empty;
        public string OpponentComment
        {
            get => _opponentComment;
            set
            {
                _opponentComment = value;
                OnPropertyChanged();
                ((Command)AddOpponentCommand).ChangeCanExecute();
            }
        }

        public ObservableCollection<OpponentNoteModel> OpponentNotes { get; private set; }

        // =========== Commands ===========
        public ICommand SaveCommand { get; }
        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }
        public ICommand AddOpponentCommand { get; }
        public ICommand RemoveOpponentCommand { get; }

        // (SaveAsync remains unchanged in this commit - will be updated in later commits)

        // =========== Helpers ===========
        private void UpdateTechniques()
        {
            Techniques = IsRandori ? DefaultRandoriTechniques()
                                   : new ObservableCollection<TechniqueScoreModel>();
        }

        private static ObservableCollection<TechniqueScoreModel> DefaultRandoriTechniques() =>
            new()
            {
                new TechniqueScoreModel { Technique = "Uchi Mata", ScoreCount = 0 },
                new TechniqueScoreModel { Technique = "Seoi Nage", ScoreCount = 0 },
                new TechniqueScoreModel { Technique = "Uki Goshi", ScoreCount = 0 }
            };

        private void RefreshTechniques()
        {
            Techniques = new ObservableCollection<TechniqueScoreModel>(Techniques);
        }

        private async Task InitCategoriesAsync()
        {
            try
            {
                var cats = await _judokaService.GetCategoriesAsync();
                Categories.Clear();
                foreach (var c in cats) Categories.Add(c);
                if (Categories.Count > 0)
                    SelectedCategory = Categories[0];
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Judokas", $"Kon categorieën niet laden: {ex.Message}", "OK");
            }
        }

        private async Task LoadJudokasByCategoryAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedCategory)) return;
                var list = await _judokaService.GetJudokasByCategoryAsync(SelectedCategory);
                Judokas.Clear();
                if (list != null)
                {
                    foreach (var j in list) Judokas.Add(j);
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Judokas", $"Kon judokas niet laden: {ex.Message}", "OK");
            }
        }

        // (Other members like SaveAsync, AddOpponentNote, etc., remain unchanged in this commit)

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
    }
}
