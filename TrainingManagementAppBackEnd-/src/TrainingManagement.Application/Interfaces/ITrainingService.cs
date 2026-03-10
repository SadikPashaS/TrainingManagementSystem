using TrainingManagement.Application.DTOs.Training;

namespace TrainingManagement.Application.Interfaces;

public interface ITrainingService
{
    Task<IEnumerable<TrainingDto>> GetAllTrainingsAsync();
    Task<TrainingDto?> GetTrainingByIdAsync(int id);
    Task<TrainingDto> CreateTrainingAsync(CreateTrainingDto createTrainingDto);
    Task UpdateTrainingAsync(int id, UpdateTrainingDto updateTrainingDto);
    Task DeleteTrainingAsync(int id);
    Task<IEnumerable<TrainingDto>> GetActiveTrainingsAsync();
}