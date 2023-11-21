using IdentityService.Extensions;
using Serilog;
using Telemetry.Logging;

var builder = WebApplication.CreateBuilder(args);
    
builder.AddCustomConfiguration();
builder.AddCustomSerilog();
builder.AddCustomViewsAndControllers();
builder.AddCustomDatabase();
builder.AddCustomOpenIddict();
builder.AddCustomIdentity();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();

var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}