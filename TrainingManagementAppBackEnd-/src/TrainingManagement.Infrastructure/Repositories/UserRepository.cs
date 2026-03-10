using Microsoft.EntityFrameworkCore;
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Interfaces;
using TrainingManagement.Infrastructure.Data;

namespace TrainingManagement.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserWithTrainingsAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.UserTrainings)
            .ThenInclude(ut => ut.Training)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<User>> GetAllUsersWithTrainingsAsync()
    {
        return await _dbSet
            .Include(u => u.UserTrainings)
            .ThenInclude(ut => ut.Training)
            .ToListAsync();
    }
}