using CommunityToolkit.Maui.Views;
using Syncfusion.Maui.Scheduler;
using Mde.Project.Mobile.Pages.Popups;
using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages;

public partial class AgendaPage : ContentPage
{
    private readonly AgendaViewModel _vm;
    private readonly AddTrainingPage _addTrainingPage;

    public AgendaPage(AgendaViewModel vm, AddTrainingPage addTrainingPage)
    {
        InitializeComponent();
        _vm = vm;
        _addTrainingPage = addTrainingPage;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private async void OnAddTrainingClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(_addTrainingPage);
    }

    private async void Scheduler_Tapped(object sender, SchedulerTappedEventArgs e)
    {
        if (e.Element is not SchedulerElement.SchedulerCell and not SchedulerElement.Appointment)
            return;

        var suggested = e.Date ?? DateTime.Now;

        var popup = new AddQuickTrainingPopup(suggested);
        var result = await this.ShowPopupAsync(popup) as AddQuickTrainingPopup.Result;
        if (result is null) return;

        try
        {
            await _vm.CreateAndAddTrainingAsync(result.Start, result.End, result.Type, result.Color);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Opslaan mislukt", ex.Message, "OK");
        }
    }
}
