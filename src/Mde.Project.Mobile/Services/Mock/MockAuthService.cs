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
    }
}
