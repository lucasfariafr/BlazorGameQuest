namespace GameServices.Controllers;

/// <summary>
/// Contrôleur API pour gérer les salles (<see cref="Room"/>).
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="RoomController"/> avec le service de salles.
    /// </summary>
    /// <param name="roomService">Le service de salles (<see cref="IRoomService"/>) utilisé pour récupérer les données.</param>
    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// Récupère toutes les salles disponibles.
    /// </summary>
    /// <returns>
    /// Un <see cref="ActionResult"/> contenant une liste de <see cref="Room"/>.
    /// Retourne un code HTTP 200 (OK) avec la liste des salles.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Room>> GetAllRooms()
    {
        var rooms = _roomService.GetAllRooms();
        return Ok(rooms);
    }

    /// <summary>
    /// Récupère une salle spécifique par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la salle à récupérer.</param>
    /// <returns>
    /// Un <see cref="ActionResult"/> contenant la <see cref="Room"/> correspondante.
    /// Retourne un code HTTP 200 (OK) si la salle existe, sinon un code HTTP 404 (Not Found) avec un message d'erreur.
    /// </returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Room> GetRoomById(int id)
    {
        var room = _roomService.GetRoomById(id);

        if (room is null)
        {
            return NotFound(new { message = $"La salle avec l'ID {id} n'existe pas." });
        }

        return Ok(room);
    }
}
