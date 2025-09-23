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
            IsBusy = true; ErrorMessage = string.Empty;

            try
            {
                // simpele lokale password-check die lijkt op Identity defaults
                bool Strong(string p) =>
                    p?.Length >= 6 && p.Any(char.IsUpper) && p.Any(char.IsLower) && p.Any(char.IsDigit);

                if (!Strong(Password))
                {
                    ErrorMessage = "Kies een sterker wachtwoord (min. 6 tekens, hoofdletter, kleine letter en cijfer).";
                    return;
                }

                var registerModel = new RegisterModel
                {
                    Username = Username?.Trim() ?? "",
                    Email = Email?.Trim() ?? "",
                    Password = Password,
                    // tijdelijk vullen om [Required] te respecteren
                    FirstName = Username?.Trim() ?? "User",
                    LastName = "-"                            // of vraag deze 2 velden in de UI
                };

                var jwt = await _authService.RegisterAsync(registerModel);
                if (jwt != null && !string.IsNullOrWhiteSpace(jwt.Token))
                {
                    await SecureStorage.SetAsync("jwt_token", jwt.Token);
                    await Shell.Current.GoToAsync("//home");
                }
                else
                {
                    ErrorMessage = "Registratie mislukt.";
                }
            }
            catch (Exception ex)
            {
                // dankzij de update zie je hier nu de precieze serverboodschap (bv. "Email already exists")
                ErrorMessage = $"Fout bij registreren: {ex.Message}";
            }
            finally { IsBusy = false; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}