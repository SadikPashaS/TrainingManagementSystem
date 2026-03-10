using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingManagement.Domain.Entities;

namespace TrainingManagement.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUserIdAsync(string userId);
    Task<User?> GetUserWithTrainingsAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersWithTrainingsAsync();
}