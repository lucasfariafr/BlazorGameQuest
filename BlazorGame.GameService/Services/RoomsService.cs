namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour gérer les salles de jeu.
/// </summary>
public class RoomsService
{
    private readonly GameDatabaseContext _context;

    /// <summary>
    /// Initialise le service avec le contexte de la base de données.
    /// </summary>
    /// <param name="context">Le contexte de la base de données du jeu.</param>
    public RoomsService(GameDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère toutes les salles de jeu.
    /// </summary>
    /// <returns>Liste de toutes les salles.</returns>
    public async Task<IReadOnlyList<Room>> GetAllRoomsAsync()
    {
        return await _context.Rooms.ToListAsync();
    }

    /// <summary>
    /// Récupère une salle de jeu par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant de la salle.</param>
    /// <returns>La salle correspondante ou null si elle n'existe pas.</returns>
    public async Task<Room?> GetRoomByIdAsync(int roomId)
    {
        return await _context.Rooms.FindAsync(roomId);
    }
}
