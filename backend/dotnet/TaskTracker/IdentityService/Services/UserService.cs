using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityService.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Services;

public interface IUserService
{
    Task<RegistrationResponse> RegisterUser(RegistrationRequest request);
    Task<LoginResponse> LoginUser(LoginRequest request);
}

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegistrationResponse> RegisterUser(RegistrationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Password != request.ConfirmPassword)
        {
            return new RegistrationResponse(null, null,
                Errors: new[] { "Password and ConfirmPassword don't match" });
        }

        var identityUser = new IdentityUser
        {
            UserName = request.Username,
            Email = request.Username
        };

        var result = await _userManager.CreateAsync(identityUser, request.Password);
        if (!result.Succeeded)
        {
            return new RegistrationResponse(null, null,
                Errors: result.Errors.Select(x => x.Description));
        }

        // TODO: Send confirmation email
        return new RegistrationResponse(request.Username, "hash", null);
    }

    public async Task<LoginResponse> LoginUser(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var user = await _userManager.FindByEmailAsync(request.Username);
        if (user == null)
        {
            return new LoginResponse(Errors: new[] { "User not found" });
        }

        var success = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!success)
        {
            return new LoginResponse(Errors: new[] { "Login failed" });
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, request.Username),
            new Claim(ClaimTypes.Name, user.Id)
        };

        var signingKey = new SymmetricSecurityKey("test--test--test"u8.ToArray());
        var token =
            new JwtSecurityToken(issuer: "paul", audience: "paul", claims: claims, expires: DateTime.Now.AddHours(1),
                signingCredentials:
                new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new LoginResponse(user.UserName, jwt);
    }
}