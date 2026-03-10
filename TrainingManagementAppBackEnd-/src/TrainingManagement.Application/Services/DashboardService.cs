using AutoMapper;
using TrainingManagement.Application.DTOs.Dashboard;
using TrainingManagement.Application.DTOs.UserTraining;
using TrainingManagement.Application.Interfaces;
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Enums;
using TrainingManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingManagement.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDashboardDto> GetUserDashboardAsync(int userId)
        {
            try
            {
                _logger.LogInformation($"========== DASHBOARD DEBUG START ==========");
                _logger.LogInformation($"Getting dashboard for user ID: {userId}");
                
                // Get user
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found");
                    throw new KeyNotFoundException($"User with ID {userId} not found");
                }

                _logger.LogInformation($"User found: {user.FirstName} {user.LastName}");
                
                // Get user trainings DIRECTLY without any filters
                var userTrainings = await _unitOfWork.UserTrainings.GetAllUserTrainingsWithDetailsAsync();
                var userTrainingsList = userTrainings?
                    .Where(ut => ut.UserId == userId)
                    .ToList() ?? new List<UserTraining>();
                
                _logger.LogInformation($"RAW TRAININGS COUNT from repository: {userTrainingsList.Count}");
                
                // Log each raw training ID
                foreach (var t in userTrainingsList)
                {
                    _logger.LogInformation($"RAW TRAINING: ID={t.Id}, TrainingId={t.TrainingId}, Status={t.Status}");
                }
                
                // Get all trainings to create a name map
                var allTrainings = await _unitOfWork.Trainings.GetAllAsync();
                var trainingNameMap = allTrainings.ToDictionary(t => t.Id, t => t.TrainingName);
                
                // Create DTOs manually
                var trainingDtos = new List<UserTrainingDto>();
                
                foreach (var training in userTrainingsList)
                {
                    string trainingName = training.Training?.TrainingName;
                    
                    if (string.IsNullOrEmpty(trainingName))
                    {
                        trainingName = trainingNameMap.GetValueOrDefault(training.TrainingId, "Unknown Training");
                    }
                    
                    string userName = training.User != null 
                        ? $"{training.User.FirstName} {training.User.LastName}".Trim() 
                        : "";
                    
                    var dto = new UserTrainingDto
                    {
                        Id = training.Id,
                        UserId = training.UserId,
                        UserName = userName,
                        TrainingId = training.TrainingId,
                        TrainingName = trainingName,
                        Status = training.Status,
                        ExpectedCompletionDate = training.ExpectedCompletionDate,
                        Comments = training.Comments,
                        AssignedAt = training.AssignedAt,
                        LastUpdatedAt = training.LastUpdatedAt
                    };
                    
                    trainingDtos.Add(dto);
                    _logger.LogInformation($"PROCESSED: TrainingId={dto.TrainingId}, Name='{dto.TrainingName}'");
                }
                
                // Calculate counts
                var pendingCount = trainingDtos.Count(t => t.Status == TrainingStatus.Pending);
                var inProgressCount = trainingDtos.Count(t => t.Status == TrainingStatus.InProgress);
                var delayedCount = trainingDtos.Count(t => t.Status == TrainingStatus.Delayed);
                var completedCount = trainingDtos.Count(t => t.Status == TrainingStatus.Completed);
                
                var dashboard = new UserDashboardDto
                {
                    WelcomeMessage = $"Welcome, {user.FirstName}!",
                    PendingCount = pendingCount,
                    InProgressCount = inProgressCount,
                    DelayedCount = delayedCount,
                    CompletedCount = completedCount,
                    MyTrainings = trainingDtos
                };

                _logger.LogInformation($"FINAL DASHBOARD - Total trainings: {trainingDtos.Count}");
                _logger.LogInformation($"========== DASHBOARD DEBUG END ==========");
                
                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting dashboard for user {userId}");
                throw;
            }
        }
    }
}