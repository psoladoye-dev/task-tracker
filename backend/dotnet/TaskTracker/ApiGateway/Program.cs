using DataAccess;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (string.IsNullOrEmpty(environment))
{
    throw new Exception("Application failed to start. Environment not specified");
}

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Log.Logger = new LoggerConfiguration()
//     .ReadFrom.Configuration(configurationRoot)
//     .Enrich.FromLogContext()
//     .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();