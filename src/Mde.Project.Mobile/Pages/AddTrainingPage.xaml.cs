using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class AddTrainingPage : ContentPage
    {
        private readonly AddTrainingViewModel _vm;

        public AddTrainingPage(AddTrainingViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var jwt = await SecureStorage.GetAsync("jwt_token") ?? string.Empty;
            _vm.SetJwt(jwt);
        }
    }
}
