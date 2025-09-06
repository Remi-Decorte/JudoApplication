using Mde.Project.WebApi.DTOs.Requests;
using Mde.Project.WebApi.DTOs.Responses;
using Mde.Project.WebApi.Entities;

namespace Mde.Project.WebApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByUsernameAsync(string username);
        string GenerateJwtToken(ApplicationUser user);
    }
}
