using Common.Configuration;
using OpenIddict.Client;
using Serilog;
using Telemetry.Logging;

namespace AuthService.Extensions;

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

    public static void AddCustomOpenIddict(this WebApplicationBuilder builder)
    {
        var openIddictBuilder = builder.Services.AddOpenIddict();
        openIddictBuilder.AddClient(options =>
        {
            options.AllowPasswordFlow();
            options.AllowClientCredentialsFlow();
            options.DisableTokenStorage();
            options.UseSystemNetHttp().SetProductInformation(typeof(Program).Assembly);
            options.AddRegistration(new OpenIddictClientRegistration
            {
                Issuer = new Uri("https://localhost:7003/", UriKind.Absolute),
            });
        });
    }
}