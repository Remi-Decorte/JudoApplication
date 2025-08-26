using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Mde.Project.Mobile.Models;
using Microsoft.Maui.Storage;

namespace Mde.Project.Mobile.ViewModels
{
    public class AddTrainingViewModel : INotifyPropertyChanged
    {
        private const string LocalFileName = "trainings_local.json";

        public AddTrainingViewModel()
        {
            Date = DateTime.Today;
            TrainingTypes = new() { "randori", "kracht", "techniek" };
            _selectedType = TrainingTypes[0];

            Techniques = DefaultRandoriTechniques();

            SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
            IncrementCommand = new Command<TechniqueScoreModel>(t =>
            {
                if (t == null) return;
                t.ScoreCount++;
                OnPropertyChanged(nameof(Techniques));
            });
            DecrementCommand = new Command<TechniqueScoreModel>(t =>
            {
                if (t == null) return;
                if (t.ScoreCount > 0) t.ScoreCount--;
                OnPropertyChanged(nameof(Techniques));
            });
        }

        // =========== Properties ===========
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        public List<string> TrainingTypes { get; }

        private string _selectedType;
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

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                (SaveCommand as Command)?.ChangeCanExecute();
            }
        }

        // =========== Commands ===========
        public ICommand SaveCommand { get; }
        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }

        // =========== Save locally to the same file as Agenda ===========
        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var all = await LoadFromLocalAsync() ?? new List<TrainingEntryModel>();

                // kies nieuw Id
                int nextId = (all.Count == 0) ? 1 : all.Max(t => t.Id) + 1;

                var entry = new TrainingEntryModel
                {
                    Id = nextId,
                    Date = Date,
                    Type = SelectedType,
                    Comment = string.Empty,
                    TechniqueScores = Techniques.ToList(),
                    Attachments = new() // leeg bij nieuwe
                };

                all.Insert(0, entry); // bovenaan tonen
                await SaveToLocalAsync(all);

                // feedback + terug
                await Application.Current?.MainPage?.DisplayAlert("Opgeslagen", "Training toegevoegd.", "OK");
                await Application.Current!.MainPage!.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Fout", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

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

        private string LocalPath => Path.Combine(FileSystem.AppDataDirectory, LocalFileName);

        private async Task SaveToLocalAsync(List<TrainingEntryModel> list)
        {
            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = false });
            await File.WriteAllTextAsync(LocalPath, json);
        }

        private async Task<List<TrainingEntryModel>?> LoadFromLocalAsync()
        {
            try
            {
                if (!File.Exists(LocalPath)) return null;
                var json = await File.ReadAllTextAsync(LocalPath);
                return JsonSerializer.Deserialize<List<TrainingEntryModel>>(json);
            }
            catch
            {
                return null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
