using Common.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentityService.DataAccess;

public class IdentityServiceDbContext : IdentityDbContext
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<IdentityServiceDbContext> _logger;
    
    public IdentityServiceDbContext(DbContextOptions<IdentityServiceDbContext> options, 
        IOptions<AppSettings> appSettings, ILogger<IdentityServiceDbContext> logger): base(options)
    {
        _logger = logger;
        ArgumentNullException.ThrowIfNull(appSettings);
        _appSettings = appSettings.Value;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        _logger.LogDebug("On model creating... {@AppSettings}", _appSettings);
    }
}