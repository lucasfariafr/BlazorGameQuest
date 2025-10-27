namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour gérer les combats entre joueurs et monstres.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class FightController : ControllerBase
{
    private readonly FightService _fightService;
    private readonly PlayerService _playerService;
    private readonly MonstersService _monsterService;

    /// <summary>
    /// Initialise le contrôleur avec les services de combat, joueur et monstre.
    /// </summary>
    public FightController(FightService fightService, PlayerService playerService, MonstersService monsterService)
    {
        _fightService = fightService;
        _playerService = playerService;
        _monsterService = monsterService;
    }

    /// <summary>
    /// Lance un combat entre un joueur et un monstre.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="monsterId">Identifiant du monstre.</param>
    /// <returns>Résultat du combat.</returns>
    [HttpPost("{playerId:int}/vs/{monsterId:int}")]
    [ProducesResponseType(typeof(FightResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fight(int playerId, int monsterId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player is null)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        var monster = await _monsterService.GetMonsterByIdAsync(monsterId);
        if (monster is null)
        {
            return NotFound(new { message = $"Le monstre avec l'ID {monsterId} n'existe pas." });
        }

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

        var result = await _fightService.ExecuteFightAsync(playerId, monsterId);

        return Ok(result);
    }

    /// <summary>
    /// Simule un combat entre un joueur et un monstre et fournit une estimation.
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