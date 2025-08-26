using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Media;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;
using System.Globalization;

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

            // Commands
            StartDictationCommand = new Command(async () => await StartDictationAsync(), () => !IsListening);
            StopDictationCommand = new Command(() => _sttCts?.Cancel(), () => IsListening);
            ApplyCommentCommand = new Command(ApplyCommentToSelected);
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
            //ErrorMessage = string.Empty;

            try
            {
                var entries = await _trainingService.GetUserTrainingEntriesAsync();
                if (entries != null)
                {
                    Trainings.Clear();
                    foreach (var entry in entries)
                    {
                        Trainings.Add(entry);
                    }
                }
                else
                {
                    //ErrorMessage = "Kon geen trainingen laden.";
                }
            }
            catch (Exception ex)
            {
                //ErrorMessage = $"Fout bij het laden van trainingen: {ex.Message}";
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
            set { _selectedTraining = value; OnPropertyChanged(); }
        }

        public ICommand StartDictationCommand { get; }
        public ICommand StopDictationCommand { get; }
        public ICommand ApplyCommentCommand { get; }

        private async Task StartDictationAsync()
        {
            try
            {
                if (IsListening) return;

                var granted = await _speech.RequestPermissions();
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
                    {
                        NewComment = result.Text.Trim();
                    }
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
            {
                UpdatePartialText(partialResult);
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => UpdatePartialText(partialResult));
            }
        }

        private void UpdatePartialText(string? partialResult)
        {
            if (!string.IsNullOrWhiteSpace(partialResult))
            {
                NewComment = partialResult.Trim();
            }

        }

        private async void ApplyCommentToSelected()
        {
            if (SelectedTraining is null)
            {
                await Application.Current?.MainPage.DisplayAlert("Selecteer", "Kies eerst een training in de lijst.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewComment))
            {
                await Application.Current?.MainPage?.DisplayAlert("Commentaar", "Voer eerst commentaar in.", "OK");
                return;
            }

            SelectedTraining.Comment = NewComment;
            NewComment = string.Empty;
            OnPropertyChanged(nameof(SelectedTraining));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
