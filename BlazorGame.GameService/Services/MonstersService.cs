namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour gérer les monstres.
/// </summary>
public class MonstersService
{
    private readonly GameDatabaseContext _context;

    /// <summary>
    /// Initialise le service avec le contexte de la base de données.
    /// </summary>
    /// <param name="context">Le contexte de la base de données du jeu.</param>
    public MonstersService(GameDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère tous les monstres.
    /// </summary>
    /// <returns>Liste de tous les monstres.</returns>
    public async Task<IReadOnlyList<Monster>> GetAllMonstersAsync()
    {
        return await _context.Monsters.ToListAsync();
    }

    /// <summary>
    /// Récupère un monstre par son identifiant.
    /// </summary>
    /// <param name="monsterId">Identifiant du monstre.</param>
    /// <returns>Le monstre ou null s'il n'existe pas.</returns>
    public async Task<Monster?> GetMonsterByIdAsync(int monsterId)
    {
        return await _context.Monsters.FindAsync(monsterId);
    }

    /// <summary>
    /// Récupère les monstres par type.
    /// </summary>
    /// <param name="type">Type de monstre.</param>
    /// <returns>Liste des monstres du type spécifié.</returns>
    public async Task<IReadOnlyList<Monster>> GetMonstersByTypeAsync(MonsterType type)
    {
        return await _context.Monsters
            .Include(m => m.Weapon)
            .Where(m => m.Type == type)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Met à jour la santé d'un monstre.
    /// </summary>
    /// <param name="monsterId">Identifiant du monstre.</param>
    /// <param name="newHealth">Nouvelle valeur de santé.</param>
    /// <returns>True si le monstre existe et a été mis à jour, sinon false.</returns>
    public async Task<bool> UpdateMonsterHealthAsync(int monsterId, double newHealth)
    {
        var monster = await _context.Monsters.FindAsync(monsterId);
        if (monster == null)
        {
            return false;
        }

        monster.Health = Math.Max(0, newHealth);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Vérifie si un monstre est en vie.
    /// </summary>
    /// <param name="monsterId">Identifiant du monstre.</param>
    /// <returns>True si le monstre est vivant, sinon false.</returns>
    public async Task<bool> IsMonsterAliveAsync(int monsterId)
    {
        var monster = await _context.Monsters
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CharacterId == monsterId);

        return monster?.Health > 0;
    }
}
