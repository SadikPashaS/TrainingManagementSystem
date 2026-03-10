using AutoMapper;
using TrainingManagement.Application.DTOs.Training;
using TrainingManagement.Application.Interfaces;
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Interfaces;

namespace TrainingManagement.Application.Services;

public class TrainingService : ITrainingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TrainingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TrainingDto>> GetAllTrainingsAsync()
    {
        var trainings = await _unitOfWork.Trainings.GetAllAsync();
        return _mapper.Map<IEnumerable<TrainingDto>>(trainings);
    }

    public async Task<TrainingDto?> GetTrainingByIdAsync(int id)
    {
        var training = await _unitOfWork.Trainings.GetByIdAsync(id);
        return training != null ? _mapper.Map<TrainingDto>(training) : null;
    }

    public async Task<TrainingDto> CreateTrainingAsync(CreateTrainingDto createTrainingDto)
    {
        var training = _mapper.Map<Training>(createTrainingDto);
        training.CreatedAt = DateTime.UtcNow;

        var createdTraining = await _unitOfWork.Trainings.AddAsync(training);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<TrainingDto>(createdTraining);
    }

    public async Task UpdateTrainingAsync(int id, UpdateTrainingDto updateTrainingDto)
    {
        var training = await _unitOfWork.Trainings.GetByIdAsync(id);
        if (training == null)
        {
            throw new KeyNotFoundException($"Training with ID {id} not found");
        }

        _mapper.Map(updateTrainingDto, training);
        await _unitOfWork.Trainings.UpdateAsync(training);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteTrainingAsync(int id)
{
    var training = await _unitOfWork.Trainings.GetByIdAsync(id);
    if (training == null)
    {
        throw new KeyNotFoundException($"Training with ID {id} not found");
    }
    
    // Check if training is assigned to any users
    var hasAssignments = await _unitOfWork.UserTrainings.ExistsAsync(ut => ut.TrainingId == id);
    if (hasAssignments)
    {
        throw new InvalidOperationException("Cannot delete training that is assigned to users");
    }
    
    // HARD DELETE - Remove from database
    await _unitOfWork.Trainings.DeleteAsync(training);
    await _unitOfWork.CompleteAsync(); // This is critical!
}

    public async Task<IEnumerable<TrainingDto>> GetActiveTrainingsAsync()
    {
        var trainings = await _unitOfWork.Trainings.GetActiveTrainingsAsync();
        return _mapper.Map<IEnumerable<TrainingDto>>(trainings);
    }
}