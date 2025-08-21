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
            StartDictationCommand = new Command(async () => await StartDictationAsync());
            StopDictationCommand = new Command(() => _sttCts?.Cancel());
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

        private string _jwtToken = string.Empty;
        public void SetJwtToken(string token) => _jwtToken = token ?? string.Empty;

        public async Task LoadTrainingsAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(_jwtToken))
                return;

            try
            {
                IsBusy = true;
                var items = await _trainingService.GetTrainingsAsync(_jwtToken);
                Trainings.Clear();
                if (items != null)
                {
                    foreach (var it in items)
                        Trainings.Add(it);
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
            set { _selectedTraining = value; OnPropertyChanged(); }
        }

        public ICommand StartDictationCommand { get; }
        public ICommand StopDictationCommand { get; }
        public ICommand ApplyCommentCommand { get; }
        private async Task StartDictationAsync()
        {
            try
            {
                var granted = await _speech.RequestPermissions(default);
                if (!granted)
                {
                    await Application.Current?.MainPage.DisplayAlert("Microfoon", "Toestemming geweigerd.", "OK");
                    return;
                }

                _sttCts?.Cancel();
                _sttCts = new CancellationTokenSource();

                // Progress ontvangt (deel)resultaten tijdens het inspreken
                var progress = new Progress<string?>(partial =>
                {
                    if (string.IsNullOrWhiteSpace(partial)) return;
                    NewComment = string.IsNullOrWhiteSpace(NewComment)
                        ? partial.Trim()
                        : $"{NewComment} {partial.Trim()}";
                });

                // Let op: deze overload vereist recognitionResult
                await _speech.ListenAsync(
                    culture: null,                 // of: new System.Globalization.CultureInfo("nl-NL")
                    recognitionResult: progress,
                    cancellationToken: _sttCts.Token
                );
            }
            catch (OperationCanceledException)
            {
                // user stopte
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage.DisplayAlert("Fout bij inspreken", ex.Message, "OK");
            }
        }


        private void ApplyCommentToSelected()
        {
            if (SelectedTraining is null)
            {
                Application.Current?.MainPage.DisplayAlert("Selecteer", "Kies eerst een training in de lijst.", "OK");
                return;
            }

            SelectedTraining.Comment = NewComment;
            OnPropertyChanged(nameof(SelectedTraining));
        }


        public Task<bool> AddTrainingAsync(string type, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(type))
                return Task.FromResult(false);

            var model = new TrainingEntryModel
            {
                Date = date,
                Type = type,
                TechniqueScores = new(),
                Comment = NewComment
            };

            Trainings.Insert(0, model);
            OnPropertyChanged(nameof(Trainings));
            return Task.FromResult(true);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
