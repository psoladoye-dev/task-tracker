using System.Diagnostics.CodeAnalysis;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using Telemetry.Logging;

namespace Telemetry.Metrics;

[ExcludeFromCodeCoverage(Justification = "Dependency injection registration")]
public static class OpenTelemetryMetricsExtensions
{
    public static void ConfigureMetrics(MeterProviderBuilder builder, OtelOptions otelOptions)
    {
        builder.AddRuntimeInstrumentation();
        builder.AddAspNetCoreInstrumentation();
        builder.AddHttpClientInstrumentation();
        builder.AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri($"{otelOptions.HttpProtobuf}/v1/metrics");
            opts.Protocol = OtlpExportProtocol.HttpProtobuf;
        });
        if (otelOptions.EnableConsoleExporter)
        {
            builder.AddConsoleExporter();
        }
    }
}