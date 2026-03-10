using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingManagement.Application.DTOs.UserTraining;

namespace TrainingManagement.Application.Interfaces;

public interface IUserTrainingService
{
    Task<IEnumerable<UserTrainingDto>> GetUserTrainingsAsync(int userId);
    Task<IEnumerable<UserTrainingDto>> GetAllUserTrainingsAsync();
    Task UpdateTrainingStatusAsync(UpdateTrainingStatusDto updateDto);
    Task AssignTrainingToUserAsync(AssignTrainingDto assignDto);
    Task RemoveTrainingFromUserAsync(int userId, int trainingId);
    Task<bool> IsTrainingAssignedToUserAsync(int userId, int trainingId);
}