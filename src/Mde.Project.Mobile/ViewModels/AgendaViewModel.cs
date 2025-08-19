using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.ViewModels
{
    public class AgendaViewModel : INotifyPropertyChanged
    {
        public AgendaViewModel(ITrainingService trainingService) // <-- inject        private readonly ITrainingService _trainingService;

        {
            _trainingService = trainingService;
        }

        public ObservableCollection<TrainingEntryModel> Trainings { get; } = new();
        public bool IsBusy { get; set; }
        private string _jwtToken = string.Empty;

        public void SetJwtToken(string token) => _jwtToken = token;

        public async Task LoadTrainingsAsync()
        {
            if (IsBusy || string.IsNullOrEmpty(_jwtToken)) return;
            IsBusy = true;

            var items = await _trainingService.GetTrainingsAsync(_jwtToken);
            Trainings.Clear();
            foreach (var it in items) Trainings.Add(it);

            IsBusy = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
