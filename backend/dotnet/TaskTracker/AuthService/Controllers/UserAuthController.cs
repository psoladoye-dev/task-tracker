using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client;

namespace AuthService.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[AllowAnonymous]
public class UserAuthController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly OpenIddictClientService _iddictClient;

    public UserAuthController(HttpClient httpClient, OpenIddictClientService iddictClient)
    {
        _httpClient = httpClient;
        _iddictClient = iddictClient;
    }
    
    [HttpPost("register", Name = "RegisterUser")]
    public async Task<IActionResult> Register()
    {
        var user = new
        {
            username = "psoladoye@gmail.com",
            password = "Password@123",
            confirmPassword = "Password@123",
        };
        var response = await _httpClient.PostAsJsonAsync("https://localhost:7003/api/v1/users/register", user);
        
        await using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<object>(stream);
        return Ok(result);
    }
    
    [HttpPost("login", Name = "LoginUser")]
    public async Task<IActionResult> Login()
    {
        var user = new
        {
            username = "psoladoye@gmail.com",
            password = "Password@123"
        };
        var response = await _iddictClient.AuthenticateWithPasswordAsync(new()
        {
            Username = user.username,
            Password = user.password
        });
        return Ok(response);
    }
}