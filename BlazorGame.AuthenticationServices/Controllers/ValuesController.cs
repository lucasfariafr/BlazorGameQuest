using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.AuthenticationServices.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ValuesController : ControllerBase
{
    [HttpGet("get-admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAdmin()
    {
        return Ok("Je suis un admin");
    }

    [HttpGet("get-player")]
    [Authorize(Roles = "Player")]
    public IActionResult GetPlayer()
    {
        return Ok("Je suis un joueur");
    }
}
