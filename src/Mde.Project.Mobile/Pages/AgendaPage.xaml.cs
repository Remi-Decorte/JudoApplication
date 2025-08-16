using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class AgendaPage : ContentPage
    {
        private readonly AgendaViewModel _vm;
        private readonly AddTrainingPage _addTrainingPage;

        // ViewModel via DI
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

            // JWT ophalen 
            var jwt = await SecureStorage.GetAsync("jwt_token") ?? string.Empty;
            _vm.SetJwtToken(jwt);

            await _vm.LoadTrainingsAsync();
        }

        // voor naar training
        private async void OnAddTrainingClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(_addTrainingPage);
        }
    }
}
