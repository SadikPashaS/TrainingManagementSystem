using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.Domain.Interfaces
{
    public interface IUserTrainingRepository : IGenericRepository<UserTraining>
    {
        Task<UserTraining?> GetUserTrainingAsync(int userId, int trainingId);
        Task<IEnumerable<UserTraining>> GetUserTrainingsAsync(int userId);
        Task<IEnumerable<UserTraining>> GetAllUserTrainingsWithDetailsAsync();
        Task UpdateTrainingStatusAsync(int userTrainingId, TrainingStatus status, DateTime? expectedDate, string? comments);
    }
}