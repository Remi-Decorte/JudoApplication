using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.Services;

namespace Mde.Project.Mobile.ViewModels;

public class AthletesViewModel : INotifyPropertyChanged
{
    private readonly JudokaService _judokaService = new();

    private string _selectedCategory = "_60";
    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory != value)
            {
                _selectedCategory = value;
                OnPropertyChanged();
                LoadJudokasAsync();
            }
        }
    }

    public ObservableCollection<string> Categories { get; set; } = new()
    {
        "_60", "-66", "-73", "-81", "-90", "-100", "+100"
    };

    public ObservableCollection<JudokaModel> Judokas { get; set; } = new();

    public AthletesViewModel()
    {
        LoadJudokasAsync();
    }

    private async void LoadJudokasAsync()
    {
        var judokas = await _judokaService.GetJudokasByCategoryAsync(SelectedCategory);

        Judokas.Clear();
        foreach (var j in judokas)
        {
            Judokas.Add(j);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

