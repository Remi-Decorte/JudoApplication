using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

        // ==== Herhaling ====
        public List<string> RecurrenceOptions { get; } =
            new() { "Geen", "Dagelijks", "Wekelijks", "Specifieke dagen" };

        private string _selectedRecurrence = "Geen";
        public string SelectedRecurrence
        {
            get => _selectedRecurrence;
            set
            {
                if (_selectedRecurrence == value) return;
                _selectedRecurrence = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCustomRecurrence));
            }
        }

        public bool IsCustomRecurrence =>
            SelectedRecurrence.Equals("Specifieke dagen", StringComparison.OrdinalIgnoreCase);

        public bool MondaySelected { get => _mondaySelected; set { _mondaySelected = value; OnPropertyChanged(); } }
        public bool TuesdaySelected { get => _tuesdaySelected; set { _tuesdaySelected = value; OnPropertyChanged(); } }
        public bool WednesdaySelected { get => _wednesdaySelected; set { _wednesdaySelected = value; OnPropertyChanged(); } }
        public bool ThursdaySelected { get => _thursdaySelected; set { _thursdaySelected = value; OnPropertyChanged(); } }
        public bool FridaySelected { get => _fridaySelected; set { _fridaySelected = value; OnPropertyChanged(); } }
        public bool SaturdaySelected { get => _saturdaySelected; set { _saturdaySelected = value; OnPropertyChanged(); } }
        public bool SundaySelected { get => _sundaySelected; set { _sundaySelected = value; OnPropertyChanged(); } }

        private bool _mondaySelected, _tuesdaySelected, _wednesdaySelected, _thursdaySelected, _fridaySelected, _saturdaySelected, _sundaySelected;

        public AddTrainingViewModel(IJudokaService judokaService)
        {
            _judokaService = judokaService;

            Date = DateTime.Today;
            TrainingTypes = new() { "Techniek", "Conditioneel", "Wedstrijdvoorbereiding", "Herstel", "Randori" };
            SelectedType = TrainingTypes[0];

            var now = DateTime.Now;
            StartTime = (Date.Date == DateTime.Today)
                ? new TimeSpan(now.Hour, (now.Minute / 15) * 15, 0)
                : new TimeSpan(9, 0, 0);
            EndTime = StartTime.Add(TimeSpan.FromHours(1));

            SelectedColor = "#1976D2"; // commit 3

            Techniques = new ObservableCollection<TechniqueScoreModel>();
            OpponentNotes = new ObservableCollection<OpponentNoteModel>();

            // Enkele, juiste declaraties:
            Categories = new ObservableCollection<string>();
            Judokas = new ObservableCollection<JudokaModel>();

            // Standaard bij randori: 1 item
            RandoriCount = 1;

            SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
            IncrementCommand = new Command<TechniqueScoreModel>(t => { if (t == null) return; t.ScoreCount++; RefreshTechniques(); });
            DecrementCommand = new Command<TechniqueScoreModel>(t => { if (t == null) return; if (t.ScoreCount > 0) t.ScoreCount--; RefreshTechniques(); });
            AddOpponentCommand = new Command(AddOpponentNote, CanAddOpponentNote);
            RemoveOpponentCommand = new Command<OpponentNoteModel>(RemoveOpponentNote);

            _ = InitCategoriesAsync();
        }

        // ===== Datum & tijd =====
        private DateTime _date;
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(); } }

        private TimeSpan _startTime;
        public TimeSpan StartTime { get => _startTime; set { _startTime = value; OnPropertyChanged(); } }

        private TimeSpan _endTime;
        public TimeSpan EndTime { get => _endTime; set { _endTime = value; OnPropertyChanged(); } }

        // ===== Kleur =====
        private string _selectedColor = "#1976D2";
        public string SelectedColor { get => _selectedColor; set { _selectedColor = value; OnPropertyChanged(); } }

        // ===== Type & Randori =====
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

                if (IsRandori)
                {
                    if (RandoriCount < 1) RandoriCount = 1;
                    EnsureOpponentNotesCount(RandoriCount);
                }
                else
                {
                    OpponentNotes.Clear();
                    RandoriCount = 0;
                }
            }
        }

        public bool IsRandori => SelectedType?.Equals("randori", StringComparison.OrdinalIgnoreCase) == true;

        // RandoriCount
        private int _randoriCount;
        public int RandoriCount
        {
            get => _randoriCount;
            set
            {
                var clamped = Math.Clamp(value, 0, 20);
                if (_randoriCount == clamped) return;
                _randoriCount = clamped;
                OnPropertyChanged();
                if (IsRandori) EnsureOpponentNotesCount(_randoriCount);
            }
        }

        private void EnsureOpponentNotesCount(int count)
        {
            if (count < 0) count = 0;
            while (OpponentNotes.Count < count)
                OpponentNotes.Add(new OpponentNoteModel { JudokaId = 0, Name = string.Empty, Comment = string.Empty });
            while (OpponentNotes.Count > count)
                OpponentNotes.RemoveAt(OpponentNotes.Count - 1);
        }

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

        private static ObservableCollection<TechniqueScoreModel> DefaultRandoriTechniques() => new()
        {
            new TechniqueScoreModel { Technique = "Uchi Mata", ScoreCount = 0 },
            new TechniqueScoreModel { Technique = "Seoi Nage", ScoreCount = 0 },
            new TechniqueScoreModel { Technique = "Uki Goshi", ScoreCount = 0 }
        };

        private void RefreshTechniques() =>
            Techniques = new ObservableCollection<TechniqueScoreModel>(Techniques);

        // ===== Busy =====
        private bool _isBusyFlag;
        public bool IsBusy
        {
            get => _isBusyFlag;
            set { _isBusyFlag = value; OnPropertyChanged(); (SaveCommand as Command)?.ChangeCanExecute(); }
        }

        // ===== Judokas & categorieën =====
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
            set { _selectedJudoka = value; OnPropertyChanged(); (AddOpponentCommand as Command)?.ChangeCanExecute(); }
        }

        private string _opponentComment = string.Empty;
        public string OpponentComment
        {
            get => _opponentComment;
            set { _opponentComment = value; OnPropertyChanged(); (AddOpponentCommand as Command)?.ChangeCanExecute(); }
        }

        public ObservableCollection<OpponentNoteModel> OpponentNotes { get; private set; }

        // ===== Commands =====
        public ICommand SaveCommand { get; }
        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }
        public ICommand AddOpponentCommand { get; }
        public ICommand RemoveOpponentCommand { get; }

        // ===== Opponent helpers =====
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
            RandoriCount = OpponentNotes.Count; // UI sync
        }

        private bool CanAddOpponentNote() =>
            SelectedJudoka != null && !string.IsNullOrWhiteSpace(OpponentComment);

        private void RemoveOpponentNote(OpponentNoteModel? note)
        {
            if (note == null) return;
            OpponentNotes.Remove(note);
            RandoriCount = OpponentNotes.Count; // UI sync
        }

        // ===== Opslaan (incl. herhaling) =====
        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var startDateTime = Date.Date.Add(StartTime);
                var endDateTime = Date.Date.Add(EndTime);
                if (endDateTime <= startDateTime) endDateTime = startDateTime.AddHours(1);

                var all = await LoadFromLocalAsync() ?? new List<TrainingEntryModel>();
                int nextId = (all.Count == 0) ? 1 : all.Max(t => t.Id) + 1;

                void AddOne(DateTime s, DateTime e, bool copyRandori)
                {
                    var entry = new TrainingEntryModel
                    {
                        Id = nextId++,
                        Date = s,
                        EndDate = e,
                        Type = SelectedType,
                        Comment = string.Empty,
                        TechniqueScores = copyRandori ? Techniques.ToList()
                                                      : (IsRandori ? new List<TechniqueScoreModel>() : Techniques.ToList()),
                        Attachments = new(),
                        OpponentNotes = (copyRandori && IsRandori) ? OpponentNotes.ToList()
                                                                   : new List<OpponentNoteModel>(),
                        Color = SelectedColor
                    };
                    all.Insert(0, entry);
                }

                // Primair item
                AddOne(startDateTime, endDateTime, copyRandori: true);

                // Reeks (30 dagen vooruit)
                if (!string.Equals(SelectedRecurrence, "Geen", StringComparison.OrdinalIgnoreCase))
                {
                    var repeatDays = BuildRepeatDaysArray(SelectedRecurrence, startDateTime.DayOfWeek,
                        MondaySelected, TuesdaySelected, WednesdaySelected, ThursdaySelected, FridaySelected, SaturdaySelected, SundaySelected);

                    var duration = endDateTime - startDateTime;
                    for (int i = 1; i <= 30; i++)
                    {
                        var d = startDateTime.Date.AddDays(i);
                        if (!repeatDays[DayOfWeekToIndex(d.DayOfWeek)]) continue;

                        var s = d + startDateTime.TimeOfDay;
                        var e = s + duration;
                        AddOne(s, e, copyRandori: false); // geen opponent-copy voor toekomstige dagen
                    }
                }

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

        private static bool[] BuildRepeatDaysArray(string recurrence, DayOfWeek startDay,
            bool mon, bool tue, bool wed, bool thu, bool fri, bool sat, bool sun)
        {
            var days = new bool[7];
            if (string.Equals(recurrence, "Dagelijks", StringComparison.OrdinalIgnoreCase))
            {
                for (int i = 0; i < 7; i++) days[i] = true;
            }
            else if (string.Equals(recurrence, "Wekelijks", StringComparison.OrdinalIgnoreCase))
            {
                days[DayOfWeekToIndex(startDay)] = true;
            }
            else if (string.Equals(recurrence, "Specifieke dagen", StringComparison.OrdinalIgnoreCase))
            {
                days[0] = mon; days[1] = tue; days[2] = wed; days[3] = thu;
                days[4] = fri; days[5] = sat; days[6] = sun;
            }
            return days;
        }

        private static int DayOfWeekToIndex(DayOfWeek d) => d switch
        {
            DayOfWeek.Monday => 0,
            DayOfWeek.Tuesday => 1,
            DayOfWeek.Wednesday => 2,
            DayOfWeek.Thursday => 3,
            DayOfWeek.Friday => 4,
            DayOfWeek.Saturday => 5,
            DayOfWeek.Sunday => 6,
            _ => 0
        };

        // ===== Lokale opslag =====
        private static string LocalPath()
        {
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
                return await JsonSerializer.DeserializeAsync<List<TrainingEntryModel>>(stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
            await JsonSerializer.SerializeAsync(stream, list, new JsonSerializerOptions { WriteIndented = true });
        }

        // ===== Categorie/Judoka helpers =====
        private async Task InitCategoriesAsync()
        {
            try
            {
                var cats = await _judokaService.GetCategoriesAsync();
                Categories.Clear();
                foreach (var c in cats) Categories.Add(c);
                if (Categories.Count > 0) SelectedCategory = Categories[0];
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
                if (list != null) foreach (var j in list) Judokas.Add(j);
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Judokas", $"Kon judokas niet laden: {ex.Message}", "OK");
            }
        }

        // ===== INotifyPropertyChanged =====
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
