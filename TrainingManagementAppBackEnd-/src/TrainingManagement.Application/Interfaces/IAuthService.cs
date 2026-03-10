using System.Threading.Tasks;
using TrainingManagement.Application.DTOs.Auth;

namespace TrainingManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task LogoutAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
    }
}