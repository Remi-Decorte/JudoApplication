using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;
using System.IO;

namespace Mde.Project.Mobile.ViewModels
{
    public class AddTrainingViewModel : INotifyPropertyChanged
    {
        private const string LocalFileName = "trainings_local.json";
        private readonly IJudokaService _judokaService;

        public AddTrainingViewModel(IJudokaService judokaService)
        {
            _judokaService = judokaService;

            Date = DateTime.Today;
            TrainingTypes = new() { "Techniek", "Conditioneel", "Wedstrijdvoorbereiding", "Herstel", "Randori" };
            SelectedType = TrainingTypes[0];  // default: Techniek

            // Default tijden (nu afgerond op kwartier, duur 1u; andere dagen: 09:00–10:00)
            var now = DateTime.Now;
            if (Date.Date == DateTime.Today)
            {
                StartTime = new TimeSpan(now.Hour, (now.Minute / 15) * 15, 0);
            }
            else
            {
                StartTime = new TimeSpan(9, 0, 0);
            }
            EndTime = StartTime.Add(TimeSpan.FromHours(1));

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

        // =========== Datum & Tijd ===========
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        private TimeSpan _startTime;
        public TimeSpan StartTime
        {
            get => _startTime;
            set { _startTime = value; OnPropertyChanged(); }
        }

        private TimeSpan _endTime;
        public TimeSpan EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(); }
        }

        // =========== Type & Randori ===========
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

                // Zorg dat bij Randori minstens 1 opponentregel bestaat
                if (IsRandori && OpponentNotes.Count == 0)
                {
                    OpponentNotes.Add(new OpponentNoteModel { JudokaId = 0, Name = string.Empty, Comment = string.Empty });
                }
                // Bij switch weg van Randori geen-opponentdata meenemen (functioneel neutraal)
                if (!IsRandori)
                {
                    OpponentNotes.Clear();
                }
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

        private void UpdateTechniques()
        {
            Techniques = IsRandori ? DefaultRandoriTechniques()
                                   : new ObservableCollection<TechniqueScoreModel>();
        }

        private static ObservableCollection<TechniqueScoreModel> DefaultRandoriTechniques() =>
            new()
            {
                new TechniqueScoreModel { Technique = "Uchi Mata",  ScoreCount = 0 },
                new TechniqueScoreModel { Technique = "Seoi Nage",  ScoreCount = 0 },
                new TechniqueScoreModel { Technique = "Uki Goshi",  ScoreCount = 0 }
            };

        private void RefreshTechniques()
        {
            Techniques = new ObservableCollection<TechniqueScoreModel>(Techniques);
        }

        // =========== Busy ===========
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
                (AddOpponentCommand as Command)?.ChangeCanExecute();
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
                (AddOpponentCommand as Command)?.ChangeCanExecute();
            }
        }

        public ObservableCollection<OpponentNoteModel> OpponentNotes { get; private set; }

        // =========== Commands ===========
        public ICommand SaveCommand { get; }
        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }
        public ICommand AddOpponentCommand { get; }
        public ICommand RemoveOpponentCommand { get; }

        // =========== Opponent helpers ===========
        private void AddOpponentNote()
        {
            if (SelectedJudoka == null) return;

            OpponentNotes.Add(new OpponentNoteModel
            {
                JudokaId = SelectedJudoka.Id,
                Name = SelectedJudoka.FullName,
                Comment = OpponentComment ?? string.Empty
            });

            OpponentComment = string.Empty;
            OnPropertyChanged(nameof(OpponentComment));
            (AddOpponentCommand as Command)?.ChangeCanExecute();
        }

        private bool CanAddOpponentNote()
        {
            return SelectedJudoka != null && !string.IsNullOrWhiteSpace(OpponentComment);
        }

        private void RemoveOpponentNote(OpponentNoteModel? note)
        {
            if (note == null) return;
            OpponentNotes.Remove(note);
        }

        // =========== Opslaan (commit 2: lokaal JSON + EndDate) ===========
        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var startDateTime = Date.Date.Add(StartTime);
                var endDateTime = Date.Date.Add(EndTime);

                // laad huidige lijst
                var all = await LoadFromLocalAsync() ?? new List<TrainingEntryModel>();

                // Voeg als nieuw item toe (commit 2 heeft nog geen edit-flow)
                int nextId = (all.Count == 0) ? 1 : all.Max(t => t.Id) + 1;

                var entry = new TrainingEntryModel
                {
                    Id = nextId,
                    Date = startDateTime,
                    EndDate = endDateTime,             // <-- commit 2
                    Type = SelectedType,
                    Comment = string.Empty,
                    TechniqueScores = Techniques.ToList(),
                    Attachments = new(),
                    OpponentNotes = IsRandori ? OpponentNotes.ToList() : new List<OpponentNoteModel>()
                };

                // bovenaan toevoegen
                all.Insert(0, entry);

                await SaveToLocalAsync(all);
                await Application.Current!.MainPage!.DisplayAlert("Opgeslagen", "Training bewaard.", "OK");
                await Application.Current!.MainPage!.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Fout", $"Kon training niet opslaan: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // =========== Lokale opslag helpers ===========
        private static string LocalPath()
        {
            // werkt overal zonder extra dependencies
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(dir, LocalFileName);
        }

        private static async Task<List<TrainingEntryModel>?> LoadFromLocalAsync()
        {
            try
            {
                var path = LocalPath();
                if (!File.Exists(path)) return new List<TrainingEntryModel>();
                using var stream = File.OpenRead(path);
                return await JsonSerializer.DeserializeAsync<List<TrainingEntryModel>>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return new List<TrainingEntryModel>();
            }
        }

        private static async Task SaveToLocalAsync(List<TrainingEntryModel> list)
        {
            var path = LocalPath();
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, list, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        // =========== Boilerplate INotifyPropertyChanged ===========
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
