namespace Telemetry.Logging;

public class OtelOptions
{
    public static string SectionKey => nameof(OtelOptions);
    public string GrpcEndpoint { get; init; } = "http://otel-collector:4317";
    public string HttpProtobuf { get; init; } = "http://otel-collector:4318";
    public string ServiceName { get; init; } = null!;
    public bool EnableConsoleExporter { get; init; }
}