namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour la gestion des combats.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class FightController : ControllerBase
{
    private readonly FightService _service;
    private readonly PlayerService _playerService;
    private readonly MonstersService _monsterService;

    public FightController(FightService service, PlayerService playerService, MonstersService monsterService)
    {
        _service = service;
        _playerService = playerService;
        _monsterService = monsterService;
    }

    /// <summary>
    /// Lance un combat entre un joueur et un monstre.
    /// </summary>
    [HttpPost("{playerId:int}/vs/{monsterId:int}")]
    [ProducesResponseType(typeof(FightResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fight(int playerId, int monsterId)
    {
        // Validation de l'existence des entités
        var playerExists = await _playerService.PlayerExistsAsync(playerId);
        if (!playerExists)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        var monsterExists = await _monsterService.MonsterExistsAsync(monsterId);
        if (!monsterExists)
        {
            return NotFound(new { message = $"Le monstre avec l'ID {monsterId} n'existe pas." });
        }

        // Validation des conditions de combat
        var isPlayerAlive = await _playerService.IsPlayerAliveAsync(playerId);
        if (!isPlayerAlive)
        {
            return BadRequest(new { message = "Le joueur est déjà mort et ne peut pas combattre." });
        }

        var isMonsterAlive = await _monsterService.IsMonsterAliveAsync(monsterId);
        if (!isMonsterAlive)
        {
            return BadRequest(new { message = "Le monstre est déjà mort." });
        }

        // Exécution du combat
        var result = await _service.ExecuteFightAsync(playerId, monsterId);

        return Ok(result);
    }

    /// <summary>
    /// Simule un combat sans sauvegarder les résultats (mode preview).
    /// </summary>
    [HttpGet("{playerId:int}/preview/{monsterId:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> SimulateFight(int playerId, int monsterId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        var monster = await _monsterService.GetMonsterByIdAsync(monsterId);
        if (monster == null)
        {
            return NotFound(new { message = $"Le monstre avec l'ID {monsterId} n'existe pas." });
        }

        // Calcul des chances de victoire
        var playerPower = player.Damage / (monster.Damage > 0 ? monster.Damage : 1);
        var winChance = Math.Min(playerPower * 50, 95);

        var preview = new
        {
            PlayerStats = new
            {
                player.Health,
                player.Damage,
                player.Armor,
                player.Strength
            },
            MonsterStats = new
            {
                Type = monster.Type.ToString(),
                monster.Health,
                monster.Damage,
                monster.Armor,
                monster.Strength
            },
            Estimation = new
            {
                WinChancePercentage = Math.Round(winChance, 1),
                PlayerAdvantage = player.Damage > monster.Damage,
                EstimatedTurns = Math.Ceiling(monster.Health / Math.Max(player.Damage - monster.Armor, 1)),
                Recommendation = winChance > 70 ? "Combat favorable" :
                                winChance > 40 ? "Combat équilibré" : "Combat risqué"
            }
        };
        
        return Ok(preview);
    }
}