using DataAccess.Entity;

namespace DataAccess.Repository;

public interface IUserRepository
{
    Task Add(ApplicationUser applicationUser);
    Task Update(ApplicationUser applicationUser);
    Task Delete(ApplicationUser applicationUser);
}

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public UserRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task Add(ApplicationUser applicationUser)
    {
        await _applicationDbContext.ApplicationUsers.AddAsync(applicationUser);
    }

    public async Task Update(ApplicationUser applicationUser)
    {
        await Task.FromResult(_applicationDbContext.ApplicationUsers.Update(applicationUser));
    }

    public async Task Delete(ApplicationUser applicationUser)
    {
        await Task.FromResult(_applicationDbContext.ApplicationUsers.Remove(applicationUser));
    }
}