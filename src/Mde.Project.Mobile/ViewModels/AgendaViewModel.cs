using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.Services;

namespace Mde.Project.Mobile.ViewModels;

public class AgendaViewModel : INotifyPropertyChanged
{
    private readonly TrainingService _trainingService = new();
    private string _jwtToken = string.Empty;

    public ObservableCollection<TrainingEntryModel> Trainings { get; set; } = new();

    public bool IsBusy { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetJwtToken(string token)
    {
        _jwtToken = token;
    }

    public async Task LoadTrainingsAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(_jwtToken))
            return;

        IsBusy = true;

        var items = await _trainingService.GetTrainingsAsync(_jwtToken);

        Trainings.Clear();
        foreach (var item in items)
            Trainings.Add(item);

        IsBusy = false;
    }

    protected void OnPropertyChanged([CallerMemberName] string name = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
