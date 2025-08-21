using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.ViewModels
{
    public class AgendaViewModel : INotifyPropertyChanged
    {
        private readonly ITrainingService _trainingService;

        public AgendaViewModel(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        public ObservableCollection<TrainingEntryModel> Trainings { get; } = new();

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        private string _jwtToken = string.Empty;

        // <-- Deze methode werd gemist in jouw foutmelding
        public void SetJwtToken(string token) => _jwtToken = token ?? string.Empty;

        // <-- Deze methode werd gemist in jouw foutmelding
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        public async Task<bool> AddTrainingAsync(string type, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(type))
                return false;

            // JWT moet al gezet zijn via SetJwtToken in OnAppearing
            var model = new TrainingEntryModel
            {
                Date = date,
                Type = type,
                TechniqueScores = new() // lege lijst
            };

            var ok = await _trainingService.AddTrainingAsync(_jwtToken, model);
            if (ok)
            {
                // Optimistisch lokaal toevoegen, bovenaan
                Trainings.Insert(0, model);
                OnPropertyChanged(nameof(Trainings));
            }
            return ok;
        }

    }
}
