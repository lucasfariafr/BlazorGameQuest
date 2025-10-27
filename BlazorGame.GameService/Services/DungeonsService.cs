namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour gérer les donjons.
/// </summary>
public class DungeonsService
{
    private readonly GameDatabaseContext _context;

    /// <summary>
    /// Initialise le service avec le contexte de la base de données.
    /// </summary>
    /// <param name="context">Le contexte de la base de données du jeu.</param>
    public DungeonsService(GameDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère tous les donjons.
    /// </summary>
    /// <returns>Liste de tous les donjons.</returns>
    public async Task<IReadOnlyList<Dungeon>> GetAllDungeonsAsync()
    {
        return await _context.Dungeons.ToListAsync();
    }

    /// <summary>
    /// Récupère un donjon par son identifiant.
    /// </summary>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Le donjon ou null s'il n'existe pas.</returns>
    public async Task<Dungeon?> GetDungeonByIdAsync(int dungeonId)
    {
        return await _context.Dungeons.FindAsync(dungeonId);
    }
}
