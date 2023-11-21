using Common.Configuration;
using IdentityService.DataAccess;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Telemetry.Logging;

namespace IdentityService.Extensions;

public static class ProgramExtensions
{
    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrEmpty(environment))
        {
            throw new Exception("Application failed to start. Environment not specified");
        }
        var configurationRoot = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        
        var appSettingsSection = configurationRoot.GetSection(AppSettings.SectionKey);
        builder.Services.AddSingleton<AppSettings>(_ => appSettingsSection.Get<AppSettings>() ?? new AppSettings());
        builder.Services.AddOptions<AppSettings>()
            .Bind(appSettingsSection)
            .ValidateDataAnnotations();
        builder.Configuration.AddConfiguration(configurationRoot).Build();
    }

    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var loggerOptions = builder.Configuration.GetSection(LoggerOptions.SectionKey)
            .Get<LoggerOptions>() ?? new LoggerOptions();

        if (loggerOptions.UseLogger == "serilog")
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateBootstrapLogger();
            builder.Host.ConfigureSerilogLogging();
        }
        else
        {
            builder.Host.ConfigureDefaultLogging();
        }
    }

    public static void AddCustomViewsAndControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void AddCustomDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<IdentityServiceDbContext>(
            options =>
            {
                options.UseInMemoryDatabase(nameof(IdentityServiceDbContext));
                options.UseOpenIddict();
            });
        builder.Services.AddHostedService<DbWorker>();
    }
    
    public static void AddCustomOpenIddict(this WebApplicationBuilder builder)
    {
        var openIddictBuilder = builder.Services.AddOpenIddict();
        openIddictBuilder.AddCore(options =>
        {
            options.UseEntityFrameworkCore()
                .UseDbContext<IdentityServiceDbContext>();
        });
        openIddictBuilder.AddServer(options =>
        {
            // options.SetAuthorizationEndpointUris("connect/authorize");
            options.SetTokenEndpointUris("connect/token");
            options.SetIntrospectionEndpointUris("connect/token/introspect");
            options.SetRevocationEndpointUris("connect/token/revoke");
            // options.SetLogoutEndpointUris("connect/logout");
            options.DisableAccessTokenEncryption();

            options.RegisterScopes("api");
            options.RegisterClaims("Something");
            
            // options.AllowPasswordFlow();
            // options.AcceptAnonymousClients();
            // options.AllowAuthorizationCodeFlow();
            options.AllowClientCredentialsFlow();
            options.AllowRefreshTokenFlow();
            
            options.AddDevelopmentEncryptionCertificate();
            options.AddDevelopmentSigningCertificate();

            options.UseAspNetCore()
            // .EnableAuthorizationEndpointPassthrough()
            // .EnableLogoutEndpointPassthrough()
            .EnableTokenEndpointPassthrough();
        });
        openIddictBuilder.AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseAspNetCore();
        });
    }
    
    public static void AddCustomIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityServiceDbContext>();
    }
    
    public static void AddCustomHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());
    }

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserService, UserService>();
    }
}