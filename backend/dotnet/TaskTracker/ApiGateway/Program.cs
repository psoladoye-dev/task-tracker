using Common.Configuration;
using Serilog;
using Telemetry;
using Telemetry.Logging;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (string.IsNullOrEmpty(environment))
{
    throw new Exception("Application failed to start. Environment not specified");
}

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var appSettings = configurationRoot.GetSection(AppSettings.SectionKey)
    .Get<AppSettings>() ?? new AppSettings();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configurationRoot)
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    var loggerOptions = builder.Configuration.GetSection(LoggerOptions.SectionKey)
        .Get<LoggerOptions>() ?? new LoggerOptions();

    if (loggerOptions.UseLogger == "serilog")
    {
        builder.Host.ConfigureSerilogLogging();
    }
    else
    {
        builder.Host.ConfigureDefaultLogging();
    }

// Add services to the container.

    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    builder.Services.AddSingleton<AppSettings>(_ => appSettings);
    builder.Services.AddOpenTelemetry(configurationRoot);

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (!app.Environment.IsEnvironment("Testing") && app.Configuration.ShouldConfigureSerilog())
    {
        app.UseSerilogRequestLogging(opts =>
        {
            opts.IncludeQueryInRequestPath = true;
        });
    }
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }