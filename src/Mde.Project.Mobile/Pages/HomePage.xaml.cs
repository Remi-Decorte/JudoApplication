using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly HomePageViewModel _vm;
        private readonly AthletesPage _athletesPage;
        private readonly AgendaPage _agendaPage;
        private readonly EventsPage _eventsPage;

        public HomePage(HomePageViewModel vm, AthletesPage athletesPage, AgendaPage agendaPage, EventsPage eventsPage)
        {
            InitializeComponent();
            _vm = vm;
            _athletesPage = athletesPage;
            _agendaPage = agendaPage;
            _eventsPage = eventsPage;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadEventsAsync();
        }

        private async void NavigateToAthletes(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_athletesPage);
        }

        private async void NavigateToAgenda(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_agendaPage);
        }
        private async void NavigateToEvents(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_eventsPage);
        }
    }
}