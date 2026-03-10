using Microsoft.EntityFrameworkCore;
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Enums;
using TrainingManagement.Domain.Interfaces;
using TrainingManagement.Infrastructure.Data;

namespace TrainingManagement.Infrastructure.Repositories
{
    public class UserTrainingRepository : GenericRepository<UserTraining>, IUserTrainingRepository
    {
        public UserTrainingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserTraining?> GetUserTrainingAsync(int userId, int trainingId)
        {
            return await _dbSet
                .Include(ut => ut.User)
                .Include(ut => ut.Training)
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TrainingId == trainingId);
        }

        public async Task<IEnumerable<UserTraining>> GetUserTrainingsAsync(int userId)
        {
            // IMPORTANT: No IsActive filter - show ALL trainings assigned to user
            return await _dbSet
                .Include(ut => ut.Training)  // Include Training to get TrainingName
                .Include(ut => ut.User)      // Include User to get UserName
                .Where(ut => ut.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserTraining>> GetAllUserTrainingsWithDetailsAsync()
        {
            // IMPORTANT: No IsActive filter - show ALL trainings
            return await _dbSet
                .Include(ut => ut.User)
                .Include(ut => ut.Training)
                .ToListAsync();
        }

        public async Task UpdateTrainingStatusAsync(int userTrainingId, TrainingStatus status, DateTime? expectedDate, string? comments)
        {
            var userTraining = await GetByIdAsync(userTrainingId);
            if (userTraining != null)
            {
                userTraining.Status = status;
                userTraining.ExpectedCompletionDate = expectedDate;
                userTraining.Comments = comments;
                userTraining.LastUpdatedAt = DateTime.UtcNow;
                await UpdateAsync(userTraining);
            }
        }
        
    }
}