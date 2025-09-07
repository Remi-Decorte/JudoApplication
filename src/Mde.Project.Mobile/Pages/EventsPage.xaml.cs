using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class EventsPage : ContentPage
    {
        private readonly EventsViewModel _vm;

        public EventsPage(EventsViewModel vm)
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
    }
}