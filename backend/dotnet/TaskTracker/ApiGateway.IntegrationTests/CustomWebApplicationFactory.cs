using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiGateway.IntegrationTests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    
}