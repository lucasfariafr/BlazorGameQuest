using BlazorGame.GameService.Services;
using BlazorGame.GameService.Helpers;
using BlazorGame.SharedModels.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour gérer les actions du joueur dans les salles.
/// Réservé aux joueurs uniquement.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = "AdminOrPlayer")]
public class ActionController : ControllerBase
{
    private readonly ActionService _actionService;
    private readonly PlayerService _playerService;

    /// <summary>
    /// Initialise le contrôleur avec le service d'actions et le service de joueurs.
    /// </summary>
    public ActionController(ActionService actionService, PlayerService playerService)
    {
        _actionService = actionService;
        _playerService = playerService;
    }

    /// <summary>
    /// Exécute l'action "Combattre" un monstre.
    /// </summary>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("fight")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fight(
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var userId = UserHelper.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Utilisateur non authentifié." });

        var player = await _playerService.GetOrCreatePlayerForUserAsync(userId);
        var result = await _actionService.FightAsync(player.CharacterId, roomId, dungeonId);
        return Ok(result);
    }

    /// <summary>
    /// Exécute l'action "Fuir" une salle.
    /// </summary>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("run-away")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RunAway(
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var userId = UserHelper.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Utilisateur non authentifié." });

        var player = await _playerService.GetOrCreatePlayerForUserAsync(userId);
        var result = await _actionService.RunAwayAsync(player.CharacterId, roomId, dungeonId);
        return Ok(result);
    }

    /// <summary>
    /// Exécute l'action "Chercher" dans une salle.
    /// </summary>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var userId = UserHelper.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Utilisateur non authentifié." });

        var player = await _playerService.GetOrCreatePlayerForUserAsync(userId);
        var result = await _actionService.SearchAsync(player.CharacterId, roomId, dungeonId);
        return Ok(result);
    }

    /// <summary>
    /// Exécute l'action "Ouvrir" un coffre.
    /// </summary>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("open")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> OpenChest(
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var userId = UserHelper.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Utilisateur non authentifié." });

        var player = await _playerService.GetOrCreatePlayerForUserAsync(userId);
        var result = await _actionService.OpenChestAsync(player.CharacterId, roomId, dungeonId);
        return Ok(result);
    }

    /// <summary>
    /// Exécute l'action "Ignorer" et passe à la salle suivante.
    /// </summary>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("ignore")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Ignore(
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var userId = UserHelper.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Utilisateur non authentifié." });

        var player = await _playerService.GetOrCreatePlayerForUserAsync(userId);
        var result = await _actionService.IgnoreAsync(player.CharacterId, roomId, dungeonId);
        return Ok(result);
    }
}
