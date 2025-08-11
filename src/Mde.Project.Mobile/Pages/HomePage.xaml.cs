using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly HomePageViewModel _vm;

        //DI 
        public HomePage(HomePageViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadEventsAsync();
        }

        private async void NavigateToAthletes(object sender, EventArgs e)
        {
            var page = Application.Current.Services.GetRequiredService<AthletesPage>();
            await Navigation.PushAsync(page);
        }

        private async void NavigateToAgenda(object sender, EventArgs e)
        {
            var page = Application.Current.Services.GetRequiredService<AgendaPage>();
            await Navigation.PushAsync(page);
        }
    }
}
