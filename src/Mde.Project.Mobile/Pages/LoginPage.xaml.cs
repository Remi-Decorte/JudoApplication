using Mde.Project.Mobile.ViewModels;

namespace Mde.Project.Mobile.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _vm;
        private readonly RegisterPage _registerPage;

        public LoginPage(LoginViewModel vm, RegisterPage registerPage)
        {
            InitializeComponent();
            _vm = vm;
            _registerPage = registerPage;
            BindingContext = _vm;
        }

        private async void OnRegisterClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(_registerPage);
        }
    }
}