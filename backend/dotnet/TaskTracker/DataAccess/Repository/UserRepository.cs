using DataAccess.Entity;

namespace DataAccess.Repository;

public interface IUserRepository
{
    Task Add(User user);
    Task Update(User user);
    Task Delete(User user);
}

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public UserRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task Add(User user)
    {
        await _applicationDbContext.Users.AddAsync(user);
    }

    public async Task Update(User user)
    {
        await Task.FromResult(_applicationDbContext.Users.Update(user));
    }

    public async Task Delete(User user)
    {
        await Task.FromResult(_applicationDbContext.Users.Remove(user));
    }
}