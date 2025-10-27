namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour gérer les API liées aux salles.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class RoomsController : ControllerBase
{
    private readonly RoomsService _roomsService;

    /// <summary>
    /// Initialise le contrôleur avec le service des salles.
    /// </summary>
    /// <param name="roomsService">Service pour gérer les salles.</param>
    public RoomsController(RoomsService roomsService)
    {
        _roomsService = roomsService;
    }

    /// <summary>
    /// Récupère toutes les salles.
    /// </summary>
    /// <returns>Liste des salles.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _roomsService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    /// <summary>
    /// Récupère une salle par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de la salle.</param>
    /// <returns>La salle correspondante ou NotFound.</returns>
    [HttpGet("{roomId:int}")]
    public async Task<IActionResult> GetById(int roomId)
    {
        var room = await _roomsService.GetRoomByIdAsync(roomId);
        if (room is null)
        {
            return NotFound(new { message = $"La salle avec l'ID {roomId} n'existe pas." });
        }

        return Ok(room);
    }
}
