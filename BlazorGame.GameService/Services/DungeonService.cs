namespace BlazorGame.GameService.Services;

public class DungeonService
{

    private readonly GameDatabaseContext _context;

    public DungeonService(GameDatabaseContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Dungeon>> GetAllAsync()
    {
        return await _context.Dungeons.ToListAsync();
    }

    public async Task<Dungeon?> GetByIdAsync(int id)
    {
        return await _context.Dungeons.FindAsync(id);
    }
}
