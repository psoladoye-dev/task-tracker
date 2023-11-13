using DataAccess.Entity;

namespace DataAccess.Repository;

public interface ITagRepository
{
    Task Add(Tag tag);
    Task Update(Tag tag);
    Task Delete(Tag tag);
}

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public TagRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task Add(Tag tag)
    {
        await _applicationDbContext.Tags.AddAsync(tag);
    }

    public async Task Update(Tag tag)
    {
        await Task.FromResult(_applicationDbContext.Tags.Update(tag));
    }

    public async Task Delete(Tag tag)
    {
        await Task.FromResult(_applicationDbContext.Tags.Remove(tag));
    }
}