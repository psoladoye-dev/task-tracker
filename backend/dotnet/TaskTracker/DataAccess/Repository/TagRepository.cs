namespace DataAccess.Repository;

public interface ITagRepository
{
    Task Add();
    Task Update();
    Task Delete();
}

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public TagRepository(ApplicationDbContext applicationDbContext)
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