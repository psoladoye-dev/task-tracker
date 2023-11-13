using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.OpenTelemetry;

namespace Telemetry.Logging;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureDefaultLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging((context, builder) =>
        {
            builder.ClearProviders();
            builder.AddConsole();
            
            builder.Services.Configure<OpenTelemetryLoggerOptions>(opt =>
            {
                opt.IncludeScopes = true;
                opt.ParseStateValues = true;
                opt.IncludeFormattedMessage = true;
            });
            
            var otelOptions = context.Configuration.GetSection(OtelOptions.SectionKey)
                .Get<OtelOptions>() ?? new OtelOptions();
            
            builder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(otelOptions.ServiceName));
                if (otelOptions.EnableConsoleExporter)
                {
                    options.AddConsoleExporter();
                }

                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                
                options.AddOtlpExporter((opt, _) =>
                {
                    opt.Endpoint = new Uri($"{otelOptions.HttpProtobuf}/v1/logs");
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
            });
        });
        return hostBuilder;
    }
    
    public static IHostBuilder ConfigureSerilogLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, config) =>
        {
            var otelOptions = context.Configuration.GetSection(OtelOptions.SectionKey)
                .Get<OtelOptions>() ?? new OtelOptions();

            config
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithSpan(new SpanOptions
                {
                    IncludeOperationName = true
                })
                .WriteTo.OpenTelemetry(opts =>
                {
                    opts.Endpoint = $"{otelOptions.HttpProtobuf}/v1/logs";
                    opts.Protocol = OtlpProtocol.HttpProtobuf;
                    opts.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = otelOptions.ServiceName
                    };
                });
        });
        return hostBuilder;
    }
}