namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour la gestion des monstres.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class MonstersController : ControllerBase
{
    private readonly MonstersService _service;

    public MonstersController(MonstersService service)
    {
        _service = service;
    }

    /// <summary>
    /// Récupère tous les monstres.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllMonsters()
    {

        var monsters = await _service.GetAllMonstersAsync();
        return Ok(monsters);
    }

    /// <summary>
    /// Récupère un monstre par son ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Monster), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMonsterById(int id)
    {
        var monster = await _service.GetMonsterByIdAsync(id);

        if (monster == null)
        {
            return NotFound(new { message = $"Le monstre avec l'ID {id} n'existe pas." });
        }

        return Ok(monster);
    }

    /// <summary>
    /// Récupère les monstres par type.
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetMonstersByType(MonsterType type)
    {
        if (!Enum.IsDefined(typeof(MonsterType), type))
        {
            return BadRequest(new { message = $"Le type de monstre '{type}' n'est pas valide." });
        }

        var monsters = await _service.GetMonstersByTypeAsync(type);
        return Ok(monsters);
    }

    /// <summary>
    /// Vérifie si un monstre est vivant.
    /// </summary>
    [HttpGet("{id:int}/is-alive")]
    public async Task<IActionResult> IsMonsterAlive(int id)
    {
        var exists = await _service.MonsterExistsAsync(id);
        if (!exists)
        {
            return NotFound(new { message = $"Le monstre avec l'ID {id} n'existe pas." });
        }

        var isAlive = await _service.IsMonsterAliveAsync(id);
        return Ok(new { monsterId = id, isAlive });
    }
}
