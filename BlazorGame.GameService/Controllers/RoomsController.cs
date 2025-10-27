namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour la gestion du joueur.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class RoomsController : ControllerBase
{
    private readonly RoomsService _service;

    public RoomsController(RoomsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _service.GetAllAsync();
        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _service.GetByIdAsync(id);
        if (room is null)
        {
            return NotFound(new { message = $"La salle avec l'ID {id} n'existe pas." });
        }

        return Ok(room);
    }
}
