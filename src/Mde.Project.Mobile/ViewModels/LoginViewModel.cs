using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        private async Task LoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var model = new LoginModel { Email = Email, Password = Password };

                // IAuthService -> JwtResponse? (MockAuthService geeft een token terug)
                var jwt = await _authService.LoginAsync(model);

                if (jwt != null && !string.IsNullOrWhiteSpace(jwt.Token))
                {
                    // bewaar veilig
                    await SecureStorage.SetAsync("jwt_token", jwt.Token);

                    // navigeer naar home (pas route aan naar jouw Shell route)
                    await Shell.Current.GoToAsync("//Home");
                }
                else
                {
                    ErrorMessage = "Login mislukt. Controleer je e-mailadres en wachtwoord.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er ging iets mis: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
