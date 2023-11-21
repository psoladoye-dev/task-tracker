using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace IdentityService.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class AuthorizationController : ControllerBase
{
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(ILogger<AuthorizationController> logger)
    {
        _logger = logger;
    }

    [HttpPost("~/connect/token"), IgnoreAntiforgeryToken]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        _logger.LogTokenRequest(request);
        
        ArgumentNullException.ThrowIfNull(request);

        if (request.IsClientCredentialsGrantType())
        {
            return await HandleClientCredentialsGrantType(request);
        }

        if (request.IsRefreshTokenGrantType())
        {
            return await HandleRefreshTokenGrantType();
        }

        return Ok();
    }

    private async Task<IActionResult> HandleRefreshTokenGrantType()
    {
        var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))
            .Principal;

        ArgumentNullException.ThrowIfNull(claimsPrincipal);
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private Task<IActionResult> HandleClientCredentialsGrantType(OpenIddictRequest request)
    {
        var clientId = request.ClientId;
        var identity = new ClaimsIdentity(authenticationType: TokenValidationParameters.DefaultAuthenticationType);

        identity.SetClaim(OpenIddictConstants.Claims.Subject, clientId);
        identity.AddClaim(new Claim("Something", "test").SetDestinations(OpenIddictConstants.Destinations.AccessToken));
        identity.SetScopes(request.GetScopes());
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult<IActionResult>(SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
    }
}