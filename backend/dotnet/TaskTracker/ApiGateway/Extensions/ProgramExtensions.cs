using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.Extensions;

public static class ProgramExtensions
{
    public static void AddCustomAuthentication(this WebApplicationBuilder builder)
    {
        var authenticationBuilder = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        authenticationBuilder.AddJwtBearer(options =>
        {
            options.Authority = "http://localhost:5043/api/v1/access-token/authorize";
            options.Audience = "ApiGateway";
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = "paul",
                ValidIssuer = "paul",
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey("test--test--test"u8.ToArray()),
                ValidateIssuerSigningKey = true
            };
        });
    }
}