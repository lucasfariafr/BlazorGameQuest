namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour la gestion du joueur.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class PlayerController : ControllerBase
{
    private readonly PlayerService _service;

    public PlayerController(PlayerService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]

    /// <summary>
    /// Récupère les informations d'un joueur.
    /// </summary>
    public async Task<IActionResult> GetPlayer(int id)
    {
        var player = await _service.GetPlayerByIdAsync(id);

        if (player == null)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {id} n'existe pas." });
        }

        return Ok(player);
    }

    /// <summary>
    /// Utilise une potion.
    /// </summary>
    [HttpPost("{playerId:int}/use-potion/{potionId:int}")]
    public async Task<IActionResult> UsePotion(int playerId, int potionId)
    {
        var player = await _service.UsePotionAsync(playerId, potionId);
        return Ok(player);
    }

    /// <summary>
    /// Vérifie si le joueur est vivant.
    /// </summary>
    [HttpGet("{id:int}/is-alive")]
    public async Task<IActionResult> IsPlayerAlive(int id)
    {
        var exists = await _service.PlayerExistsAsync(id);
        if (!exists)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {id} n'existe pas." });
        }

        var isAlive = await _service.IsPlayerAliveAsync(id);
        return Ok(new { playerId = id, isAlive });
    }

    /// <summary>
    /// Équipe une arme au joueur.
    /// </summary>
    [HttpPost("{playerId:int}/equip-weapon/{weaponId:int}")]
    public async Task<IActionResult> EquipWeapon(int playerId, int weaponId)
    {
        var result = await _service.EquipWeaponAsync(playerId, weaponId);

        if (!result)
        {
            return NotFound(new { message = $"Le joueur {playerId} ou l'arme {weaponId} n'existe pas." });
        }

        return Ok(new { message = "Arme équipée avec succès.", playerId, weaponId });
    }

    /// <summary>
    /// Ajoute une potion à l'inventaire du joueur.
    /// </summary>
    [HttpPost("{playerId:int}/add-potion")]
    public async Task<IActionResult> AddPotion(int playerId, [FromBody] Potion potion)
    {

        if (potion == null)
        {
            return BadRequest(new { message = "Les données de la potion sont invalides." });
        }

        var result = await _service.AddPotionAsync(playerId, potion);

        if (!result)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        return Ok(new { message = "Potion ajoutée avec succès.", playerId, potionType = potion.Type });
    }
}
