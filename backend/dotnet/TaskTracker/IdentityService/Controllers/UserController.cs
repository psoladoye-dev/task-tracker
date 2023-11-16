using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

public record RegistrationRequest(
    [Required]
    [StringLength(100)]
    string Username, 
    [Required]
    [StringLength(100)]
    string Password, 
    [Required]
    [StringLength(100)]
    string ConfirmPassword);

public record RegistrationResponse(string? Username, string? PasswordHash, IEnumerable<string>? Errors);
public record LoginRequest(string Username, string Password);
public record  LoginResponse(string Username);

[ApiController]
[Route("api/v1/users")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register", Name = "RegisterUser")]
    public async Task<ActionResult> Register(RegistrationRequest request)
    {
        var response = await _userService.RegisterUser(request);
        if (response.Errors != null)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    [HttpPost("login", Name = "LoginUser")]
    public async Task<ActionResult> Login(LoginRequest loginRequest)
    {
        return Ok(new {});
    }
}