using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class AgendaPage : ContentPage
    {
        private readonly AgendaViewModel _vm;

        public AgendaPage(AgendaViewModel vm) // dependency injection dit 
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var jwt = Preferences.Get("jwt_token", string.Empty);
            _vm.SetJwtToken(jwt);
            await _vm.LoadTrainingsAsync();
        }
    }
}

