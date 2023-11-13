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

    public Task<int> SaveChanges()
    {
        throw new NotImplementedException();
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