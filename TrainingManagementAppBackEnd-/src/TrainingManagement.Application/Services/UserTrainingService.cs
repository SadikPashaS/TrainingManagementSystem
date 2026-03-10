using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrainingManagement.Application.DTOs.UserTraining;
using TrainingManagement.Application.Interfaces;
using TrainingManagement.Application.Exceptions;  // Added this
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Interfaces;

namespace TrainingManagement.Application.Services
{
    public class UserTrainingService : IUserTrainingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserTrainingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserTrainingDto>> GetUserTrainingsAsync(int userId)
        {
            var userTrainings = await _unitOfWork.UserTrainings.GetUserTrainingsAsync(userId);
            
            // Map to DTO and ensure TrainingName is included
            var dtos = _mapper.Map<IEnumerable<UserTrainingDto>>(userTrainings);
            
            // Manually set TrainingName if AutoMapper didn't
            foreach (var dto in dtos)
            {
                var userTraining = userTrainings.FirstOrDefault(ut => ut.Id == dto.Id);
                if (userTraining?.Training != null)
                {
                    dto.TrainingName = userTraining.Training.TrainingName;
                }
            }
            
            return dtos;
        }

        public async Task<IEnumerable<UserTrainingDto>> GetAllUserTrainingsAsync()
        {
            var userTrainings = await _unitOfWork.UserTrainings.GetAllUserTrainingsWithDetailsAsync();
            
            // Map to DTO and ensure TrainingName is included
            var dtos = _mapper.Map<IEnumerable<UserTrainingDto>>(userTrainings);
            
            // Manually set TrainingName if AutoMapper didn't
            foreach (var dto in dtos)
            {
                var userTraining = userTrainings.FirstOrDefault(ut => ut.Id == dto.Id);
                if (userTraining?.Training != null)
                {
                    dto.TrainingName = userTraining.Training.TrainingName;
                }
            }
            
            return dtos;
        }

        public async Task UpdateTrainingStatusAsync(UpdateTrainingStatusDto updateDto)
        {
            var userTraining = await _unitOfWork.UserTrainings.GetByIdAsync(updateDto.UserTrainingId);
            if (userTraining == null)
            {
                throw new KeyNotFoundException($"User training with ID {updateDto.UserTrainingId} not found");
            }

            // Validate that ExpectedCompletionDate is provided when status is Delayed
            if (updateDto.Status == Domain.Enums.TrainingStatus.Delayed && !updateDto.ExpectedCompletionDate.HasValue)
            {
                throw new ValidationException("Expected Completion Date is required when status is Delayed");
            }

            userTraining.Status = updateDto.Status;
            userTraining.ExpectedCompletionDate = updateDto.ExpectedCompletionDate;
            userTraining.Comments = updateDto.Comments;
            userTraining.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.UserTrainings.UpdateAsync(userTraining);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AssignTrainingToUserAsync(AssignTrainingDto assignDto)
        {
            if (await IsTrainingAssignedToUserAsync(assignDto.UserId, assignDto.TrainingId))
            {
                throw new InvalidOperationException("Training already assigned to this user");
            }

            var userTraining = _mapper.Map<UserTraining>(assignDto);
            await _unitOfWork.UserTrainings.AddAsync(userTraining);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveTrainingFromUserAsync(int userId, int trainingId)
        {
            var userTraining = await _unitOfWork.UserTrainings.GetUserTrainingAsync(userId, trainingId);
            if (userTraining == null)
            {
                throw new KeyNotFoundException("Training assignment not found");
            }

            await _unitOfWork.UserTrainings.DeleteAsync(userTraining);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> IsTrainingAssignedToUserAsync(int userId, int trainingId)
        {
            return await _unitOfWork.UserTrainings.ExistsAsync(ut => 
                ut.UserId == userId && ut.TrainingId == trainingId);
        }
    }
}