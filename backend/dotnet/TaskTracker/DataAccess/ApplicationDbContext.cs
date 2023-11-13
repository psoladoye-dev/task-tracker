using Common.Configuration;
using DataAccess.Entity;
using DataAccess.Extensions;
using DataAccess.HardCoded;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DataAccess;

public class ApplicationDbContext : DbContext
{
    private readonly AppSettings _appSettings;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IOptions<AppSettings> appSettings) : base(options)
    {
        ArgumentNullException.ThrowIfNull(appSettings);
        _appSettings = appSettings.Value;
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<UserTask> Tasks => Set<UserTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_appSettings.ApplicationDbSchema);
        modelBuilder.SeedEnumItems<StatusType, StatusTypeEnum>(x => x);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var timeNowUtc = DateTime.UtcNow;
        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity.Entity is IEntity entity)
            {
                switch (changedEntity.State)
                {
                    case EntityState.Added:
                    {
                        entity.CreatedAt = timeNowUtc;
                        entity.UpdatedAt = timeNowUtc;
                        break;
                    }
                    case EntityState.Modified:
                    {
                        Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                        entity.UpdatedAt = timeNowUtc;
                        break;
                    }
                }
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}