using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/test")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class TestController : ControllerBase
{
    [HttpGet]
    public Task<ActionResult> Test()
    {
        return Task.FromResult<ActionResult>(Ok(new { message = "Success" }));
    }
}