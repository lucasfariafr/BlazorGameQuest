namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour la gestion des monstres.
/// </summary>
public class MonstersService
{
    private readonly GameDatabaseContext _context;

    public MonstersService(GameDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère un monstre par son ID avec ses relations.
    /// </summary>
    public async Task<Monster?> GetMonsterByIdAsync(int monsterId)
    {
        return await _context.Monsters
            .Include(m => m.Weapon)
            .FirstOrDefaultAsync(m => m.CharacterId == monsterId);
    }

    /// <summary>
    /// Récupère tous les monstres.
    /// </summary>
    public async Task<IReadOnlyList<Monster>> GetAllMonstersAsync()
    {
        return await _context.Monsters
            .Include(m => m.Weapon)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Récupère les monstres par type.
    /// </summary>
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
    /// Vérifie si un monstre existe.
    /// </summary>
    public async Task<bool> MonsterExistsAsync(int monsterId)
    {
        return await _context.Monsters.AnyAsync(m => m.CharacterId == monsterId);
    }

    /// <summary>
    /// Vérifie si un monstre est vivant.
    /// </summary>
    public async Task<bool> IsMonsterAliveAsync(int monsterId)
    {
        var monster = await _context.Monsters
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CharacterId == monsterId);
        
        return monster?.Health > 0;
    }
}