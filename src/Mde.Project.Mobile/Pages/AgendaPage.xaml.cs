using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages;

public partial class AgendaPage : ContentPage
{
    private AgendaViewModel ViewModel => (AgendaViewModel)BindingContext;

    public AgendaPage(string jwtToken)
    {
        InitializeComponent();
        ViewModel.SetJwtToken(jwtToken);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadTrainingsAsync();
    }
}
