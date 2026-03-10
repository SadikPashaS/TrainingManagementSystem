using System.Threading.Tasks;
using TrainingManagement.Application.DTOs.Dashboard;

namespace TrainingManagement.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<UserDashboardDto> GetUserDashboardAsync(int userId);
    }
}