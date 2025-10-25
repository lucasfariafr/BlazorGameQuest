namespace GameServices.Services;

/// <summary>
/// Service gérant les salles du jeu (<see cref="Room"/>).
/// </summary>
public class RoomService : IRoomService
{
    private readonly IReadOnlyList<Room> _rooms;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="RoomService"/> avec un initialisateur de salles.
    /// </summary>
    /// <param name="roomInitializer">L'initialisateur de salles (<see cref="IRoomInitializer"/>) qui fournit les données initiales.</param>
    public RoomService(IRoomInitializer roomInitializer)
    {
        _rooms = roomInitializer.InitializeRooms();
    }

    /// <summary>
    /// Récupère toutes les salles disponibles dans le jeu.
    /// </summary>
    /// <returns>
    /// Une liste en lecture seule (<see cref="IReadOnlyList{Room}"/>) contenant toutes les salles.
    /// </returns>
    public IReadOnlyList<Room> GetAllRooms() => _rooms;

    /// <summary>
    /// Récupère une salle spécifique par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la salle à récupérer.</param>
    /// <returns>
    /// La salle correspondante (<see cref="Room"/>), ou <see langword="null"/> si aucune salle ne correspond à l'identifiant.
    /// </returns>
    public Room? GetRoomById(int id) => _rooms.FirstOrDefault(r => r.RoomId == id);
}
