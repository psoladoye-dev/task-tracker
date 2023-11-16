using Common.Configuration;
using IdentityService.DataAccess;
using IdentityService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
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
                options.UseInMemoryDatabase("IdentityServiceDb");
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
            options.SetAuthorizationEndpointUris("connect/authorize");
            options.SetTokenEndpointUris("connect/token");
            options.SetLogoutEndpointUris("connect/logout");
            options.AllowAuthorizationCodeFlow();
            options.AllowClientCredentialsFlow();
            options.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();
            options.UseAspNetCore()
                .EnableAuthorizationEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
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
            // .AddDefaultTokenProviders();
        var authenticationBuilder = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        authenticationBuilder.AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = "paul",
                ValidIssuer = "paul",
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey("This is a test key"u8.ToArray()),
                ValidateIssuerSigningKey = true
            };
        });
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