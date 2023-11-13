using System.Diagnostics.CodeAnalysis;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using Telemetry.Logging;

namespace Telemetry.Tracing;

[ExcludeFromCodeCoverage(Justification = "Dependency injection registration")]
public static class OpenTelemetryTracingExtensions
{
    private static readonly string[] IgnoredTraceEndpoints =
    {
        "/metrics",
        "/health",
        "/v1/traces",
        "/v1/metrics",
        "/v1/logs",
    };
    public static void ConfigureTracing(TracerProviderBuilder builder, OtelOptions otelOptions)
    {
        builder.SetSampler(new AlwaysOnSampler());
        builder.AddSource("*");
        builder.SetErrorStatusOnException();
        builder.AddAspNetCoreInstrumentation(opts =>
        {
            opts.RecordException = true;
            opts.Filter = context => !IgnoredTraceEndpoints.Contains(context.Request.Path.Value);
        });
        builder.AddHttpClientInstrumentation(opts =>
        {
            opts.RecordException = true;
            opts.FilterHttpRequestMessage =
                message => !IgnoredTraceEndpoints.Contains(message.RequestUri?.PathAndQuery);
        });
        builder.AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri($"{otelOptions.HttpProtobuf}/v1/traces");
            opts.Protocol = OtlpExportProtocol.HttpProtobuf;
        });
        if (otelOptions.EnableConsoleExporter)
        {
            builder.AddConsoleExporter();
        }
    }
}