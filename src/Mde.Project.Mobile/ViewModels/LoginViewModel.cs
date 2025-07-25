using Mde.Project.Mobile.Models;
using Mde.Project.Mobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mde.Project.Mobile.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService = new();

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            var loginModel = new LoginModel
            {
                Email = Email,
                Password = Password
            };

            var token = await _authService.LoginAsync(loginModel);

            if (!string.IsNullOrEmpty(token))
            {
                // Bewaar token en navigeer
                Preferences.Set("jwt", token);
                await Shell.Current.GoToAsync("//home"); // ← we maken dit straks goed met route
            }
            else
            {
                ErrorMessage = "Login mislukt. Controleer je gegevens.";
            }

            IsBusy = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
