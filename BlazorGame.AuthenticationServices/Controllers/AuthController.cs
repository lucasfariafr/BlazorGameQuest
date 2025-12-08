using BlazorGame.AuthenticationServices.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.AuthenticationServices.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly RequestLoggerMiddleware _requestLoggerMiddleware;

    private readonly ILogger<AuthController> _logger;

    public AuthController(RequestLoggerMiddleware requestLoggerMiddleware, ILogger<AuthController> logger)
    {
        _requestLoggerMiddleware = requestLoggerMiddleware;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation($"les informations de connexions sont { request.Username} ... { request.Password}");
            var token = await _requestLoggerMiddleware.LoginAsync(request.Username, request.Password);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}