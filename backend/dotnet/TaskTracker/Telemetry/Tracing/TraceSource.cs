using System.Diagnostics;
using Common.Configuration;

namespace Telemetry.Tracing;

public interface ITraceSource : IDisposable
{
    ActivitySource ActivitySource { get; }
}

public class TraceSource : ITraceSource
{
    public TraceSource(AppSettings appSettings)
    {
        ActivitySource = new ActivitySource(appSettings.ServiceName, "1.0.0");
    }
    public ActivitySource ActivitySource { get; }
    
    public void Dispose() => ActivitySource.Dispose();
}