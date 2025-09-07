using Mde.Project.Mobile.ViewModels;
using Microsoft.Maui.Storage;
using System;

namespace Mde.Project.Mobile.Pages
{
    public partial class AgendaPage : ContentPage
    {
        private readonly AgendaViewModel _vm;

        // ViewModel via DI
        public AgendaPage(AgendaViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // JWT ophalen (leeg string als niet aanwezig)
            var jwt = await SecureStorage.GetAsync("jwt_token") ?? string.Empty;
            _vm.SetJwtToken(jwt);

            await _vm.LoadTrainingsAsync();
        }

        // voor naar training
        private async void OnAddTrainingClicked(object? sender, EventArgs e)
        {
            await DisplayAlert("Nwcht", "pagina add page wordt nog gemakat", "Ok");
        }
    }
}
