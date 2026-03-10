using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingManagement.Domain.Entities;

namespace TrainingManagement.Domain.Interfaces;

public interface ITrainingRepository : IGenericRepository<Training>
{
    Task<Training?> GetTrainingWithUsersAsync(int trainingId);
    Task<IEnumerable<Training>> GetActiveTrainingsAsync();
}