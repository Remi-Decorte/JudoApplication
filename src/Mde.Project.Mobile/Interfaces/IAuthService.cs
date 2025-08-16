using Mde.Project.Mobile.Models;

namespace Mde.Project.Mobile.Interfaces
{
    public interface IAuthService
    {
        Task<JwtResponse?> LoginAsync(LoginModel model);
        Task LogoutAsync();
    }
}
