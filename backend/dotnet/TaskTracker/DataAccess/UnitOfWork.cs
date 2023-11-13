using DataAccess.Repository;
using Microsoft.Extensions.Logging;

namespace DataAccess;

public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    ITaskRepository TaskRepository { get; }
    ITagRepository TagRepository { get; }

    Task<int> SaveChanges();
}

public class UnitOfWork : IUnitOfWork
{
    private bool _disposed;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly ApplicationDbContext _applicationDbContext;

    public IUserRepository UserRepository { get; }
    public ITaskRepository TaskRepository { get; }
    public ITagRepository TagRepository { get; }

    public UnitOfWork(ILogger<UnitOfWork> logger, ApplicationDbContext applicationDbContext,
        IUserRepository userRepository, ITaskRepository taskRepository, ITagRepository tagRepository)
    {
        _logger = logger;
        _applicationDbContext = applicationDbContext;
        UserRepository = userRepository;
        TaskRepository = taskRepository;
        TagRepository = tagRepository;
    }

    public async Task<int> SaveChanges()
    {
        await using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();
        try
        {
            await OnBeforeSavingChanges();
            var numOfRowsAffected = await _applicationDbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return numOfRowsAffected;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to save changes. Transaction rolled back");
            throw;
        }
    }

    private Task OnBeforeSavingChanges()
    {
        // TODO: Audit database changes
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _applicationDbContext.Dispose();
        _disposed = true;
    }
}