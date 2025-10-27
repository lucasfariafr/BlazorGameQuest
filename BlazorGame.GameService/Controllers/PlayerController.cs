namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour gérer les API liées aux joueurs.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class PlayerController : ControllerBase
{
    private readonly PlayerService _playerService;

    /// <summary>
    /// Initialise le contrôleur avec le service des joueurs.
    /// </summary>
    /// <param name="playerService">Service pour gérer les joueurs.</param>
    public PlayerController(PlayerService playerService)
    {
        _playerService = playerService;
    }

    /// <summary>
    /// Récupère tous les joueurs.
    /// </summary>
    /// <returns>Liste des joueurs.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllPlayers()
    {
        var players = await _playerService.GetAllPlayersAsync();
        return Ok(players);
    }

    /// <summary>
    /// Récupère un joueur par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant du joueur.</param>
    /// <returns>Le joueur ou NotFound.</returns>
    [HttpGet("{playerId:int}")]
    public async Task<IActionResult> GetPlayer(int playerId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);

        if (player == null)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        return Ok(player);
    }

    /// <summary>
    /// Permet à un joueur d'utiliser une potion.
    /// </summary>
    [HttpPost("{playerId:int}/use-potion/{potionId:int}")]
    public async Task<IActionResult> UsePotion(int playerId, int potionId)
    {
            try
    {
        var player = await _playerService.UsePotionAsync(playerId, potionId);
        return Ok(player);
    }
    catch (KeyNotFoundException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    }

    /// <summary>
    /// Vérifie si un joueur est vivant.
    /// </summary>
    [HttpGet("{playerId:int}/is-alive")]
    public async Task<IActionResult> IsPlayerAlive(int playerId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player is null)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        var isAlive = await _playerService.IsPlayerAliveAsync(playerId);
        return Ok(new { playerId, isAlive });
    }

    /// <summary>
    /// Équipe une arme pour un joueur.
    /// </summary>
    [HttpPost("{playerId:int}/equip-weapon/{weaponId:int}")]
    public async Task<IActionResult> EquipWeapon(int playerId, int weaponId)
    {
        var result = await _playerService.EquipWeaponAsync(playerId, weaponId);

        if (!result)
        {
            return NotFound(new { message = $"Le joueur {playerId} ou l'arme {weaponId} n'existe pas." });
        }

        return Ok(new { message = "Arme équipée avec succès.", playerId, weaponId });
    }

    /// <summary>
    /// Ajoute une potion à un joueur.
    /// </summary>
    [HttpPost("{playerId:int}/add-potion")]
    public async Task<IActionResult> AddPotion(int playerId, [FromBody] Potion potion)
    {
        if (potion == null)
        {
            return BadRequest(new { message = "Les données de la potion sont invalides." });
        }

        var result = await _playerService.AddPotionAsync(playerId, potion);

        if (!result)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        return Ok(new { message = "Potion ajoutée avec succès.", playerId, potionType = potion.Type });
    }
}
