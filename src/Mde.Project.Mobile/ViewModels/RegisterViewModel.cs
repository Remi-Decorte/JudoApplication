using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
            RegisterCommand = new Command(async () => await RegisterAsync(), () => !IsBusy);
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
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

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); (RegisterCommand as Command)?.ChangeCanExecute(); }
        }

        public ICommand RegisterCommand { get; }

        private async Task RegisterAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var registerModel = new RegisterModel
                {
                    Username = Username,
                    Email = Email,
                    Password = Password,
                    FirstName = string.Empty,
                    LastName = string.Empty
                };

                var jwtResponse = await _authService.RegisterAsync(registerModel);

                if (jwtResponse != null && !string.IsNullOrWhiteSpace(jwtResponse.Token))
                {
                    // Store the JWT token securely
                    await SecureStorage.SetAsync("jwt_token", jwtResponse.Token);
                    // Navigate to the home page
                    await Shell.Current.GoToAsync("//home");
                }
                else
                {
                    ErrorMessage = "Registratie mislukt. Controleer je gegevens.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Fout bij registreren: {ex.Message}";
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
