namespace BlazorGame.GameService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class DungeonController : ControllerBase
{
    private readonly DungeonService _service;

    public DungeonController(DungeonService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dungeons = await _service.GetAllAsync();
        return Ok(dungeons);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dungeon = await _service.GetByIdAsync(id);
        if (dungeon is null)
        {
            return NotFound(new { message = $"Le donjon avec l'ID {id} n'existe pas." });
        }

        return Ok(dungeon);
    }
}
