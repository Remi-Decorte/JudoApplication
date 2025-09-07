using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.ViewModels
{
    public class AthleteDetailViewModel : INotifyPropertyChanged
    {
        private readonly JudokaModel _judoka;
        private const string LocalFileName = "trainings_local.json";

        public AthleteDetailViewModel(JudokaModel judoka)
        {
            _judoka = judoka;
            FullName = judoka.FullName;

            Notes = new ObservableCollection<AthleteNoteModel>();
            _ = LoadNotesAsync();
            AddNoteCommand = new Command(async () => await AddNoteAsync(), CanAddNote);
        }

        public string FullName { get; }
        public ObservableCollection<AthleteNoteModel> Notes { get; }

        private string _newComment = string.Empty;
        public string NewComment
        {
            get => _newComment;
            set
            {
                _newComment = value;
                OnPropertyChanged();
                (AddNoteCommand as Command)?.ChangeCanExecute();
            }
        }

        public ICommand AddNoteCommand { get; }

        private bool CanAddNote() =>
            !string.IsNullOrWhiteSpace(NewComment);

        private async Task AddNoteAsync()
        {
            var note = new AthleteNoteModel
            {
                Date = DateTime.Today,
                Comment = NewComment.Trim()
            };
            Notes.Add(note);
            NewComment = string.Empty;

            await SaveNotesAsync();
        }

        // 🔹 Laden: filter alle trainingen op deze judoka
        private async Task LoadNotesAsync()
        {
            try
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, LocalFileName);
                if (!File.Exists(path)) return;

                var json = await File.ReadAllTextAsync(path);
                var trainings = JsonSerializer.Deserialize<List<TrainingEntryModel>>(json);
                if (trainings == null) return;

                Notes.Clear();
                foreach (var t in trainings)
                {
                    if (t.OpponentNotes == null) continue;
                    foreach (var note in t.OpponentNotes.Where(n => n.JudokaId == _judoka.Id))
                    {
                        Notes.Add(new AthleteNoteModel
                        {
                            Date = t.Date,
                            Comment = note.Comment
                        });
                    }
                }
            }
            catch { /* negeer fouten */ }
        }

        // 🔹 Opslaan: herlaadt training-file en voegt comments toe
        private async Task SaveNotesAsync()
        {
            try
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, LocalFileName);
                if (!File.Exists(path)) return;

                var json = await File.ReadAllTextAsync(path);
                var trainings = JsonSerializer.Deserialize<List<TrainingEntryModel>>(json);
                if (trainings == null) return;

                // update laatste training van vandaag of maak nieuwe
                var training = trainings.FirstOrDefault(t => t.Date.Date == DateTime.Today.Date);
                if (training == null)
                {
                    training = new TrainingEntryModel
                    {
                        Id = (trainings.Count == 0) ? 1 : trainings.Max(t => t.Id) + 1,
                        Date = DateTime.Today,
                        Type = "randori",
                        TechniqueScores = new(),
                        Attachments = new(),
                        OpponentNotes = new List<OpponentNoteModel>()
                    };
                    trainings.Add(training);
                }

                training.OpponentNotes ??= new List<OpponentNoteModel>();
                foreach (var note in Notes)
                {
                    if (!training.OpponentNotes.Any(n => n.Comment == note.Comment && n.JudokaId == _judoka.Id))
                    {
                        training.OpponentNotes.Add(new OpponentNoteModel
                        {
                            JudokaId = _judoka.Id,
                            Name = _judoka.FullName,
                            Comment = note.Comment
                        });
                    }
                }

                json = JsonSerializer.Serialize(trainings, new JsonSerializerOptions { WriteIndented = false });
                await File.WriteAllTextAsync(path, json);
            }
            catch { /* negeer fouten */ }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
