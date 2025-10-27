namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour gérer les API liées aux monstres.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class MonstersController : ControllerBase
{
    private readonly MonstersService _monsterService;

    /// <summary>
    /// Initialise le contrôleur avec le service des monstres.
    /// </summary>
    /// <param name="monsterService">Service pour gérer les monstres.</param>
    public MonstersController(MonstersService monsterService)
    {
        _monsterService = monsterService;
    }

    /// <summary>
    /// Récupère tous les monstres.
    /// </summary>
    /// <returns>Liste des monstres.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllMonsters()
    {
        var monsters = await _monsterService.GetAllMonstersAsync();
        return Ok(monsters);
    }

    /// <summary>
    /// Récupère un monstre par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant du monstre.</param>
    /// <returns>Le monstre ou NotFound.</returns>
    [HttpGet("{monsterId:int}")]
    [ProducesResponseType(typeof(Monster), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMonsterById(int monsterId)
    {
        var monster = await _monsterService.GetMonsterByIdAsync(monsterId);

        if (monster == null)
        {
            return NotFound(new { message = $"Le monstre avec l'ID {monsterId} n'existe pas." });
        }

        return Ok(monster);
    }

    /// <summary>
    /// Récupère les monstres par type.
    /// </summary>
    /// <param name="type">Type de monstre.</param>
    /// <returns>Liste des monstres du type spécifié.</returns>
    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetMonstersByType(MonsterType type)
    {
        if (!Enum.IsDefined(typeof(MonsterType), type))
        {
            return BadRequest(new { message = $"Le type de monstre '{type}' n'est pas valide." });
        }

        var monsters = await _monsterService.GetMonstersByTypeAsync(type);
        return Ok(monsters);
    }

    /// <summary>
    /// Vérifie si un monstre est vivant.
    /// </summary>
    /// <param name="id">Identifiant du monstre.</param>
    /// <returns>True si le monstre est vivant, sinon NotFound.</returns>
    [HttpGet("{monsterId:int}/is-alive")]
    public async Task<IActionResult> IsMonsterAlive(int monsterId)
    {
        var monster = await _monsterService.GetMonsterByIdAsync(monsterId);
        if (monster is null)
        {
            return NotFound(new { message = $"Le monstre avec l'ID {monsterId} n'existe pas." });
        }

        var isAlive = await _monsterService.IsMonsterAliveAsync(monsterId);
        return Ok(new { monsterId, isAlive });
    }
}
