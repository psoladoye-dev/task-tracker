using OpenIddict.Abstractions;

namespace IdentityService.Controllers;

public static partial class LogMessages
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Critical,
        Message = "Registration payload {Request}")]
    public static partial void LogRegistrationRequest(this ILogger logger, RegistrationRequest request);
    
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Connect Token Request {Request}")]
    public static partial void LogTokenRequest(this ILogger logger, OpenIddictRequest? request);
}