using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.Services.Interfaces;

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

            // randori default technieken dat ze moeten uit kiezen voorlopig
            // kan nog uitbreiden dat ze zelf technieken kunnen schrijven
            Techniques = new ObservableCollection<TechniqueScoreModel>
            {
                new TechniqueScoreModel { Technique = "Uchi Mata", ScoreCount = 0 },
                new TechniqueScoreModel { Technique = "Seoi Nage", ScoreCount = 0 },
                new TechniqueScoreModel { Technique = "Uki Goshi", ScoreCount = 0 }
            };

            SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
            IncrementCommand = new Command<TechniqueScoreModel>(t => { if (t != null) { t.ScoreCount++; OnPropertyChanged(nameof(Techniques)); }});
            DecrementCommand = new Command<TechniqueScoreModel>(t => { if (t != null && t.ScoreCount > 0) { t.ScoreCount--; OnPropertyChanged(nameof(Techniques)); }});
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
                if (_selectedType == value) return;
                _selectedType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRandori));
            }
        }

        public bool IsRandori => SelectedType?.Equals("randori", StringComparison.OrdinalIgnoreCase) == true;

        public ObservableCollection<TechniqueScoreModel> Techniques { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); (SaveCommand as Command)?.ChangeCanExecute(); }
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
                var model = new TrainingEntryModel
                {
                    Date = this.Date,
                    Type = this.SelectedType,
                    TechniqueScores = IsRandori ? Techniques.ToList() : new List<TechniqueScoreModel>()
                };

                var ok = await _trainingService.CreateTrainingAsync(model, _jwtToken);
                if (ok)
                {
                    await Application.Current!.MainPage.DisplayAlert("Geslaagd", "Training opgeslagen.", "Ok");
                    await Application.Current!.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current!.MainPage.DisplayAlert("Fout", "Opslaan mislukt.", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage.DisplayAlert("Fout", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
