namespace DataAccess.Repository;

public interface ITaskRepository
{
    Task Add();
    Task Update();
    Task Delete();
}

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public TaskRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public Task Add()
    {
        throw new NotImplementedException();
    }

    public Task Update()
    {
        throw new NotImplementedException();
    }

    public Task Delete()
    {
        throw new NotImplementedException();
    }
}