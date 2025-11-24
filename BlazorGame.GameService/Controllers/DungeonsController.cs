using BlazorGame.GameService.Services;
using BlazorGame.SharedModels.Enums.Environment;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour gérer les API liées aux donjons.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class DungeonController : ControllerBase
{
    private readonly DungeonsService _dungeonService;

    /// <summary>
    /// Initialise le contrôleur avec le service des donjons.
    /// </summary>
    /// <param name="dungeonService">Service pour gérer les donjons.</param>
    public DungeonController(DungeonsService dungeonService)
    {
        _dungeonService = dungeonService;
    }

    /// <summary>
    /// Récupère tous les donjons.
    /// </summary>
    /// <returns>Liste des donjons.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllDungeons()
    {
        var dungeons = await _dungeonService.GetAllDungeonsAsync();
        return Ok(dungeons);
    }

    /// <summary>
    /// Récupère un donjon par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant du donjon.</param>
    /// <returns>Le donjon correspondant ou NotFound.</returns>
    [HttpGet("{dungeonId:int}")]
    public async Task<IActionResult> GetDungeonById(int dungeonId)
    {
        var dungeon = await _dungeonService.GetDungeonByIdAsync(dungeonId);
        if (dungeon is null)
        {
            return NotFound(new { message = $"Le donjon avec l'ID {dungeonId} n'existe pas." });
        }

        return Ok(dungeon);
    }

    /// <summary>
    /// Génère un donjon aléatoire avec des salles, monstres et coffres.
    /// </summary>
    /// <param name="level">Niveau de difficulté (Easy, Medium, Difficult).</param>
    /// <param name="roomCount">Nombre de salles à générer (par défaut 5).</param>
    /// <returns>Le donjon généré.</returns>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateRandomDungeon(
        [FromQuery] DungeonLevel level = DungeonLevel.Easy,
        [FromQuery] int roomCount = 5)
    {
        if (roomCount < 1 || roomCount > 20)
        {
            return BadRequest(new { message = "Le nombre de salles doit être entre 1 et 20." });
        }

        var dungeon = await _dungeonService.GenerateRandomDungeonAsync(level, roomCount);
        return CreatedAtAction(nameof(GetDungeonById), new { dungeonId = dungeon.DungeonId }, dungeon);
    }
}
