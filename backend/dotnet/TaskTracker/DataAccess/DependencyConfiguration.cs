using System.Diagnostics.CodeAnalysis;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

[ExcludeFromCodeCoverage(Justification = "Dependency injection registration")]
public static class DependencyConfiguration
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opts =>
        {
            opts.UseNpgsql(configuration.GetConnectionString("ApplicationDbConnection"));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        return services;
    }
}