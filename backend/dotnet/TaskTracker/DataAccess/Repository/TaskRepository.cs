using DataAccess.Entity;

namespace DataAccess.Repository;

public interface ITaskRepository
{
    Task Add(UserTask userTask);
    Task Update(UserTask userTask);
    Task Delete(UserTask userTask);
}

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public TaskRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task Add(UserTask userTask)
    {
        await _applicationDbContext.Tasks.AddAsync(userTask);
    }

    public async Task Update(UserTask userTask)
    {
        await Task.FromResult(_applicationDbContext.Tasks.Update(userTask));
    }

    public async Task Delete(UserTask userTask)
    {
        await Task.FromResult(_applicationDbContext.Tasks.Remove(userTask));
    }
}