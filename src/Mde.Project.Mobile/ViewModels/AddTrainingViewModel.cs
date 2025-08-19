using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.Services;

namespace Mde.Project.Mobile.ViewModels
{
    public class AddTrainingViewModel : INotifyPropertyChanged
    {
        private readonly ITrainingService _trainingService;
        private string _jwtToken = string.Empty;

        public AddTrainingViewModel(ITrainingService trainingService)
        {
            _trainingService = trainingService;

            // defaults
            Date = DateTime.Today;
            TrainingTypes = new() { "randori", "kracht", "techniek" };
            SelectedType = TrainingTypes[0];

            // randori default technieken
            Techniques = new ObservableCollection<TechniqueScoreModel>
            {
                new TechniqueScoreModel { Technique = "Uchi Mata", ScoreCount = 0 },
                new TechniqueScoreModel { Technique = "Seoi Nage", ScoreCount = 0 },
                new TechniqueScoreModel { Technique = "Uki Goshi", ScoreCount = 0 }
            };

            SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
            IncrementCommand = new Command<TechniqueScoreModel>(t => { if (t != null) { t.ScoreCount++; OnPropertyChanged(nameof(Techniques)); } });
            DecrementCommand = new Command<TechniqueScoreModel>(t => { if (t != null && t.ScoreCount > 0) { t.ScoreCount--; OnPropertyChanged(nameof(Techniques)); } });
        }

        public void SetJwt(string token) => _jwtToken = token;

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        public List<string> TrainingTypes { get; }

        private string _selectedType = "randori";
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                OnPropertyChanged();
                UpdateTechniques();
            }
        }

        private ObservableCollection<TechniqueScoreModel> _techniques;
        public ObservableCollection<TechniqueScoreModel> Techniques
        {
            get => _techniques;
            set { _techniques = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }

        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var training = new TrainingEntryModel
                {
                    Date = this.Date,
                    Type = this.SelectedType,
                    TechniqueScores = this.Techniques.ToList()
                };

                await _trainingService.CreateTrainingAsync(training, _jwtToken);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateTechniques()
        {
            if (SelectedType == "randori")
            {
                Techniques = new ObservableCollection<TechniqueScoreModel>
                {
                    new TechniqueScoreModel { Technique = "Uchi Mata", ScoreCount = 0 },
                    new TechniqueScoreModel { Technique = "Seoi Nage", ScoreCount = 0 },
                    new TechniqueScoreModel { Technique = "Uki Goshi", ScoreCount = 0 }
                };
            }
            else
            {
                Techniques = new ObservableCollection<TechniqueScoreModel>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
