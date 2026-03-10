using Microsoft.EntityFrameworkCore;
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Interfaces;
using TrainingManagement.Infrastructure.Data;

namespace TrainingManagement.Infrastructure.Repositories;

public class TrainingRepository : GenericRepository<Training>, ITrainingRepository
{
    public TrainingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Training?> GetTrainingWithUsersAsync(int trainingId)
    {
        return await _dbSet
            .Include(t => t.UserTrainings)
            .ThenInclude(ut => ut.User)
            .FirstOrDefaultAsync(t => t.Id == trainingId);
    }

    public async Task<IEnumerable<Training>> GetActiveTrainingsAsync()
    {
        return await _dbSet
            .Where(t => t.IsActive)
            .ToListAsync();
    }
}