using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Telemetry.Logging;
using Telemetry.Metrics;
using Telemetry.Tracing;

namespace Telemetry;

[ExcludeFromCodeCoverage(Justification = "Dependency injection registration")]
public static class DependencyConfiguration
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services,
        IConfiguration configuration)
    {
        var otelOptions = configuration.GetSection(OtelOptions.SectionKey)
            .Get<OtelOptions>() ?? new OtelOptions();

        services.AddSingleton<ITraceSource, TraceSource>();
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(otelOptions.ServiceName))
            .WithTracing(builder => OpenTelemetryTracingExtensions.ConfigureTracing(builder, otelOptions))
            .WithMetrics(builder => OpenTelemetryMetricsExtensions.ConfigureMetrics(builder, otelOptions));
        return services;
    }
}