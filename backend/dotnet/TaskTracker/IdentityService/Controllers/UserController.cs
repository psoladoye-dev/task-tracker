using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

public record RegistrationRequest(
    [Required] [StringLength(100)] string Username,
    [Required] [StringLength(100)] string Password,
    [Required] [StringLength(100)] string ConfirmPassword);

public record RegistrationResponse(string? Username = null, string? PasswordHash = null,
    IEnumerable<string>? Errors = null);

public record LoginRequest(string Username, string Password);

public record LoginResponse(string? Username = null, string? AccessToken = null, IEnumerable<string>? Errors = null);

[ApiController]
[Route("api/v1/users")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register", Name = "RegisterUser")]
    public async Task<IActionResult> Register(RegistrationRequest request)
    {
        _logger.LogRegistrationRequest(request);
        var response = await _userService.RegisterUser(request);
        if (response.Errors != null)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}