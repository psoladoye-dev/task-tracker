using IdentityService.Controllers;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public interface IUserService
{
    Task<RegistrationResponse> RegisterUser(RegistrationRequest request);
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
}