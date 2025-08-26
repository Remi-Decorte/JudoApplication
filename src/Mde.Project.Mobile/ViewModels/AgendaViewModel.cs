using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;                
using Microsoft.Maui.Controls;      
using Microsoft.Maui.Media;       
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui.Media;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.ViewModels
{
    public class AgendaViewModel : INotifyPropertyChanged
    {
        private readonly ITrainingService _trainingService;

        // Speech-to-text
        private readonly ISpeechToText _speech = SpeechToText.Default;
        private CancellationTokenSource? _sttCts;

        public AgendaViewModel(ITrainingService trainingService)
        {
            _trainingService = trainingService;

            // Voice to text
            StartDictationCommand = new Command(async () => await StartDictationAsync(), () => !IsListening);
            StopDictationCommand = new Command(() => _sttCts?.Cancel(), () => IsListening);
            ApplyCommentCommand = new Command(ApplyCommentToSelected);

            // Snelle lokale foto (geen service)
            QuickShotCommand = new Command(async () => await QuickShotAsync());

            // ?? NIEUW: selecteer blok/kaartje op tap
            SelectTrainingCommand = new Command<TrainingEntryModel?>(t =>
            {
                if (t == null) return;
                SelectedTraining = t;
            });
        }

        // ====== Collections & state ======
        public ObservableCollection<TrainingEntryModel> Trainings { get; } = new();

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

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
                var entries = await _trainingService.GetUserTrainingEntriesAsync();
                Trainings.Clear();
                if (entries != null)
                {
                    foreach (var entry in entries)
                        Trainings.Add(entry);
                }
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
            set { _newComment = value; OnPropertyChanged(); }
        }

        private TrainingEntryModel? _selectedTraining;
        public TrainingEntryModel? SelectedTraining
        {
            get => _selectedTraining;
            set
            {
                _selectedTraining = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartDictationCommand { get; }
        public ICommand StopDictationCommand { get; }
        public ICommand ApplyCommentCommand { get; }
        public ICommand QuickShotCommand { get; }
        public ICommand SelectTrainingCommand { get; } // ?? nieuw

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
                        cancellationToken: _sttCts.Token
                    );

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
            if (MainThread.IsMainThread)
                UpdatePartialText(partialResult);
            else
                MainThread.BeginInvokeOnMainThread(() => UpdatePartialText(partialResult));
        }

        private void UpdatePartialText(string? partialResult)
        {
            if (!string.IsNullOrWhiteSpace(partialResult))
                NewComment = partialResult.Trim();
        }

        private async void ApplyCommentToSelected()
        {
            if (SelectedTraining is null)
            {
                await Application.Current?.MainPage.DisplayAlert(
                    "Selecteer",
                    "Kies eerst een training in de lijst.",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewComment))
            {
                await Application.Current?.MainPage?.DisplayAlert(
                    "Commentaar",
                    "Voer eerst commentaar in.",
                    "OK");
                return;
            }

            SelectedTraining.Comment = NewComment;
            NewComment = string.Empty;
            OnPropertyChanged(nameof(SelectedTraining));
        }

        // ====== Snelle lokale foto ======
        private async Task QuickShotAsync()
        {
            try
            {
                if (SelectedTraining == null)
                {
                    await Application.Current?.MainPage?.DisplayAlert(
                        "Geen training geselecteerd",
                        "Selecteer eerst een training in de lijst om er een foto aan toe te voegen.",
                        "OK");
                    return;
                }

                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Application.Current?.MainPage?.DisplayAlert(
                        "Camera",
                        "Foto nemen wordt niet ondersteund op dit toestel.",
                        "OK");
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

                await Application.Current?.MainPage?.DisplayAlert(
                    "Foto toegevoegd",
                    "De foto is lokaal opgeslagen en aan de training gekoppeld.",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert(
                    "Foto",
                    $"Kon geen foto opslaan: {ex.Message}",
                    "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
