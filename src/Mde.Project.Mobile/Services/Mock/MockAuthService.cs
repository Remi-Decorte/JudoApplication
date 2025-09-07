using Mde.Project.Mobile.Interfaces;
using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Services.Mock
{
    public class MockAuthService : IAuthService
    {
        public Task<JwtResponse?> LoginAsync(LoginModel model)
        {
            // Simuleer geslaagde login met fake token
            return Task.FromResult<JwtResponse?>(new JwtResponse { Token = "mock-token" });
        }

        public Task LogoutAsync()
        {
            // Niets te doen in mock
            return Task.CompletedTask;
        }

        public Task<JwtResponse?> RegisterAsync(RegisterModel register)
        {
            var ok = !string.IsNullOrWhiteSpace(register.Email) &&
                     !string.IsNullOrWhiteSpace(register.Password) &&
                     !string.IsNullOrWhiteSpace(register.Username);
            return Task.FromResult(ok ? new JwtResponse { Token = "mock-token" } : null);
        }
    }
}
