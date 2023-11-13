namespace Telemetry.Logging;

public class LoggerOptions
{
    public static string SectionKey => nameof(LoggerOptions);

    public string UseLogger { get; init; } = "default";
}