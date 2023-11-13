using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace Telemetry.Logging;

[ExcludeFromCodeCoverage(Justification = "Dependency injection registration")]
public static class ConfigurationExtensions
{
    public static bool ShouldConfigureSerilog(this IConfiguration configuration)
    {
        var loggerOptions = configuration.GetSection(LoggerOptions.SectionKey)
            .Get<LoggerOptions>() ?? new LoggerOptions();
        if (string.IsNullOrEmpty(loggerOptions.UseLogger))
        {
            return false;
        }

        return loggerOptions.UseLogger.ToLower() == "serilog";
    }
    
}