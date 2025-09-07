using Mde.Project.Mobile.ViewModels;
using Microsoft.Maui.Storage;

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
            try
            {
                var jwt = await SecureStorage.GetAsync("jwt_token") ?? string.Empty;
                _vm.SetJwt(jwt);
            }
            catch
            {
                // SecureStorage kan onbeschikbaar zijn; stil negeren
            }
        }
    }
}
