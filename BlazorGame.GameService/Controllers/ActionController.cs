using BlazorGame.GameService.Services;
using BlazorGame.SharedModels.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour gérer les actions du joueur dans les salles.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ActionController : ControllerBase
{
    private readonly ActionService _actionService;

    /// <summary>
    /// Initialise le contrôleur avec le service d'actions.
    /// </summary>
    public ActionController(ActionService actionService)
    {
        _actionService = actionService;
    }

    /// <summary>
    /// Exécute l'action "Combattre" un monstre.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("fight")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fight(
        [FromQuery] int playerId,
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var result = await _actionService.FightAsync(playerId, roomId, dungeonId);
        return Ok(result);
    }

    /// <summary>
    /// Exécute l'action "Fuir" une salle.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("run-away")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RunAway(
        [FromQuery] int playerId,
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var result = await _actionService.RunAwayAsync(playerId, roomId, dungeonId);
        return Ok(result);
    }

    /// <summary>
    /// Exécute l'action "Chercher" dans une salle.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] int playerId,
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var result = await _actionService.SearchAsync(playerId, roomId, dungeonId);
        return Ok(result);
    }

    /// <summary>
    /// Exécute l'action "Ouvrir" un coffre.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("open")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> OpenChest(
        [FromQuery] int playerId,
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var result = await _actionService.OpenChestAsync(playerId, roomId, dungeonId);
        return Ok(result);
    }

    /// <summary>
    /// Exécute l'action "Ignorer" et passe à la salle suivante.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="roomId">Identifiant de la salle.</param>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Résultat de l'action.</returns>
    [HttpPost("ignore")]
    [ProducesResponseType(typeof(ActionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Ignore(
        [FromQuery] int playerId,
        [FromQuery] int roomId,
        [FromQuery] int dungeonId)
    {
        var result = await _actionService.IgnoreAsync(playerId, roomId, dungeonId);
        return Ok(result);
    }
}
