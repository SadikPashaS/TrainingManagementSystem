using TrainingManagement.Domain.Interfaces;
using TrainingManagement.Infrastructure.Data;

namespace TrainingManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public IUserRepository Users { get; }
    public ITrainingRepository Trainings { get; }
    public IUserTrainingRepository UserTrainings { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IUserRepository userRepository,
        ITrainingRepository trainingRepository,
        IUserTrainingRepository userTrainingRepository)
    {
        _context = context;
        Users = userRepository;
        Trainings = trainingRepository;
        UserTrainings = userTrainingRepository;
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public Task BeginTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync()
    {
        return Task.CompletedTask;
    }
}