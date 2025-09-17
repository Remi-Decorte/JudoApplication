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
        private int? _editingId = null;
        private bool _isBusy;

        public AddTrainingViewModel(IJudokaService judokaService)
        {
            _judokaService = judokaService;

            Date = DateTime.Today;
            TrainingTypes = new() { "randori", "kracht", "techniek" };
            _selectedType = TrainingTypes[0];

            Techniques = DefaultRandoriTechniques();

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

            // laad categorieën + eventueel default judokas
            _ = InitCategoriesAsync();
        }

        // ctor voor "Bewerk training"
        public AddTrainingViewModel(IJudokaService judokaService, TrainingEntryModel trainingToEdit) : this(judokaService)
        {
            _editingId = trainingToEdit.Id;
            Date = trainingToEdit.Date;
            SelectedType = trainingToEdit.Type;

            Techniques = new ObservableCollection<TechniqueScoreModel>(
                trainingToEdit.TechniqueScores ?? new List<TechniqueScoreModel>()
            );

            OpponentNotes = new ObservableCollection<OpponentNoteModel>(
                trainingToEdit.OpponentNotes ?? new List<OpponentNoteModel>()
            );
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

        private bool CanAddOpponentNote() =>
            IsRandori && SelectedJudoka != null && !string.IsNullOrWhiteSpace(OpponentComment);

        private void AddOpponentNote()
        {
            if (!CanAddOpponentNote()) return;
            OpponentNotes.Add(new OpponentNoteModel
            {
                JudokaId = SelectedJudoka!.Id,
                Name = SelectedJudoka!.FullName,
                Comment = OpponentComment.Trim()
            });
            OpponentComment = string.Empty;
        }

        private void RemoveOpponentNote(OpponentNoteModel? note)
        {
            if (note == null) return;
            OpponentNotes.Remove(note);
        }

        // =========== Data laad helpers ===========
        private async Task InitCategoriesAsync()
        {
            try
            {
                var cats = await _judokaService.GetCategoriesAsync();
                Categories.Clear();
                foreach (var c in cats) Categories.Add(c);
                if (Categories.Count > 0)
                    SelectedCategory = Categories[0]; // trigger judokas load
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

        // =========== Save ===========
        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var all = await LoadFromLocalAsync() ?? new List<TrainingEntryModel>();

                if (_editingId.HasValue)
                {
                    var existing = all.FirstOrDefault(t => t.Id == _editingId.Value);
                    if (existing != null)
                    {
                        existing.Date = Date;
                        existing.Type = SelectedType;
                        existing.TechniqueScores = Techniques.ToList();
                        existing.OpponentNotes = OpponentNotes.ToList();
                    }
                }
                else
                {
                    int nextId = (all.Count == 0) ? 1 : all.Max(t => t.Id) + 1;
                    var entry = new TrainingEntryModel
                    {
                        Id = nextId,
                        Date = Date,
                        Type = SelectedType,
                        Comment = string.Empty,
                        TechniqueScores = Techniques.ToList(),
                        Attachments = new(),
                        OpponentNotes = OpponentNotes.ToList()
                    };
                    all.Insert(0, entry);
                }

                await SaveToLocalAsync(all);
                await Application.Current?.MainPage?.DisplayAlert("Opgeslagen", "Training bewaard.", "OK");
                await Application.Current!.MainPage!.Navigation.PopAsync();
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

        private void RefreshTechniques()
        {
            Techniques = new ObservableCollection<TechniqueScoreModel>(Techniques);
        }

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
//fix