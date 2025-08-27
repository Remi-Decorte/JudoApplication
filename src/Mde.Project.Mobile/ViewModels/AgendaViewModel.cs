using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Media;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;
using Microsoft.Maui;
using Microsoft.Maui.Media;    // ✅ voor MediaPicker
using Microsoft.Maui.Storage;  // ✅ voor FileSystem

namespace Mde.Project.Mobile.ViewModels
{
    public class AgendaViewModel : INotifyPropertyChanged
    {
        private readonly ITrainingService _trainingService;
        private readonly IJudokaService _judokaService;
        private const string LocalFileName = "trainings_local.json";

        private readonly ISpeechToText _speech = SpeechToText.Default;
        private CancellationTokenSource? _sttCts;

        public AgendaViewModel(ITrainingService trainingService, IJudokaService judokaService)
        {
            _trainingService = trainingService;
            _judokaService = judokaService;

            StartDictationCommand = new Command(async () => await StartDictationAsync(), () => !IsListening);
            StopDictationCommand = new Command(() => _sttCts?.Cancel(), () => IsListening);
            SaveCommentCommand = new Command(async () => await SaveCommentAsync(), CanSaveComment);
            QuickShotCommand = new Command(async () => await QuickShotAsync());
            SelectTrainingCommand = new Command<TrainingEntryModel?>(t =>
            {
                if (t == null) return;
                SelectedTraining = t;
            });

            // ✅ nieuw: property bestaat nu ook
            EditTrainingCommand = new Command(async () => await EditTrainingAsync(), () => SelectedTraining != null);
        }

        // ====== Collections & state ======
        public ObservableCollection<TrainingEntryModel> Trainings { get; } = new();

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); } }

        private bool _isListening;
        public bool IsListening
        {
            get => _isListening;
            set
            {
                _isListening = value;
                OnPropertyChanged();
                ((Command)StartDictationCommand).ChangeCanExecute();
                ((Command)StopDictationCommand).ChangeCanExecute();
            }
        }

        public async Task LoadTrainingsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var local = await LoadFromLocalAsync();
                if (local == null || local.Count == 0)
                {
                    var fromService = await _trainingService.GetUserTrainingEntriesAsync();
                    local = fromService?.ToList() ?? new System.Collections.Generic.List<TrainingEntryModel>();
                    await SaveToLocalAsync(local);
                }

                Trainings.Clear();
                foreach (var entry in local)
                    Trainings.Add(entry);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ====== Comment flow ======
        private string _newComment = string.Empty;
        public string NewComment
        {
            get => _newComment;
            set
            {
                _newComment = value;
                OnPropertyChanged();
                ((Command)SaveCommentCommand).ChangeCanExecute();
            }
        }

        private TrainingEntryModel? _selectedTraining;
        public TrainingEntryModel? SelectedTraining
        {
            get => _selectedTraining;
            set
            {
                _selectedTraining = value;
                NewComment = _selectedTraining?.Comment ?? string.Empty;
                OnPropertyChanged();
                ((Command)SaveCommentCommand).ChangeCanExecute();
                ((Command)EditTrainingCommand).ChangeCanExecute();   // ✅ enable/disable “Bewerk” knop
            }
        }

        public ICommand StartDictationCommand { get; }
        public ICommand StopDictationCommand { get; }
        public ICommand SaveCommentCommand { get; }
        public ICommand QuickShotCommand { get; }
        public ICommand SelectTrainingCommand { get; }
        public ICommand EditTrainingCommand { get; }   // ✅ ontbrekende property toegevoegd

        private bool CanSaveComment() =>
            SelectedTraining != null && !string.IsNullOrWhiteSpace(NewComment);

        private async Task SaveCommentAsync()
        {
            if (!CanSaveComment()) return;

            SelectedTraining!.Comment = NewComment.Trim();
            OnPropertyChanged(nameof(SelectedTraining));
            await SaveToLocalAsync(Trainings.ToList());

            await Application.Current?.MainPage?.DisplayAlert("Opgeslagen",
                "Je comment is bewaard en blijft beschikbaar bij volgende opstart.", "OK");
        }

        private async Task QuickShotAsync()
        {
            try
            {
                if (SelectedTraining == null)
                {
                    await Application.Current?.MainPage?.DisplayAlert("Geen training geselecteerd",
                        "Selecteer eerst een training in de lijst om er een foto aan toe te voegen.", "OK");
                    return;
                }

                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Application.Current?.MainPage?.DisplayAlert("Camera",
                        "Foto nemen wordt niet ondersteund op dit toestel.", "OK");
                    return;
                }

                FileResult? result = await MediaPicker.CapturePhotoAsync();
                if (result == null) return;

                using Stream source = await result.OpenReadAsync();
                string fileName = $"training_{SelectedTraining.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string destPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                using FileStream dest = File.OpenWrite(destPath);
                await source.CopyToAsync(dest);
                await dest.FlushAsync();

                SelectedTraining.Attachments ??= new System.Collections.Generic.List<TrainingAttachmentModel>();
                SelectedTraining.Attachments.Add(new TrainingAttachmentModel
                {
                    Type = "photo",
                    Uri = destPath,
                    FileName = Path.GetFileName(destPath)
                });

                OnPropertyChanged(nameof(SelectedTraining));
                await SaveToLocalAsync(Trainings.ToList());

                await Application.Current?.MainPage?.DisplayAlert("Foto toegevoegd",
                    "De foto is lokaal opgeslagen en aan de training gekoppeld.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Foto", $"Kon geen foto opslaan: {ex.Message}", "OK");
            }
        }

        private string LocalPath => Path.Combine(FileSystem.AppDataDirectory, LocalFileName);

        private async Task SaveToLocalAsync(System.Collections.Generic.List<TrainingEntryModel> list)
        {
            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = false });
            await File.WriteAllTextAsync(LocalPath, json);
        }

        private async Task<System.Collections.Generic.List<TrainingEntryModel>?> LoadFromLocalAsync()
        {
            try
            {
                if (!File.Exists(LocalPath)) return null;
                var json = await File.ReadAllTextAsync(LocalPath);
                return JsonSerializer.Deserialize<System.Collections.Generic.List<TrainingEntryModel>>(json);
            }
            catch { return null; }
        }

        // ====== STT ======
        private async Task StartDictationAsync()
        {
            try
            {
                if (IsListening) return;

                bool granted = await _speech.RequestPermissions();
                if (!granted)
                {
                    await Application.Current?.MainPage?.DisplayAlert("Microfoon", "Toestemming geweigerd.", "OK");
                    return;
                }

                _sttCts?.Cancel();
                _sttCts?.Dispose();
                _sttCts = new CancellationTokenSource();
                IsListening = true;

                try
                {
                    var result = await _speech.ListenAsync(
                        culture: CultureInfo.CurrentCulture,
                        recognitionResult: new Progress<string?>(OnSpeechRecognitionUpdate),
                        cancellationToken: _sttCts.Token);

                    if (!string.IsNullOrWhiteSpace(result.Text))
                        NewComment = result.Text.Trim();
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    await Application.Current?.MainPage?.DisplayAlert("Fout bij het inspreken", $"Onverwachte fout: {ex.Message}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Fout", $"Onverwachte fout: {ex.Message}", "OK");
            }
            finally
            {
                IsListening = false;
                _sttCts?.Dispose();
                _sttCts = null;
            }
        }

        private void OnSpeechRecognitionUpdate(string? partialResult)
        {
            if (MainThread.IsMainThread) UpdatePartialText(partialResult);
            else MainThread.BeginInvokeOnMainThread(() => UpdatePartialText(partialResult));
        }

        private void UpdatePartialText(string? partialResult)
        {
            if (!string.IsNullOrWhiteSpace(partialResult))
                NewComment = partialResult.Trim();
        }

        private async Task EditTrainingAsync()
        {
            if (SelectedTraining == null)
            {
                await Application.Current?.MainPage?.DisplayAlert("Geen selectie", "Kies eerst een training om te bewerken.", "OK");
                return;
            }

            await Application.Current?.MainPage?.Navigation.PushAsync(
                new Pages.AddTrainingPage(
                    new AddTrainingViewModel(_judokaService, SelectedTraining)   
                )
            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}