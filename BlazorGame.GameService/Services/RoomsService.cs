namespace BlazorGame.GameService.Services;

public class RoomsService
{

    private readonly GameDatabaseContext _context;

    public RoomsService(GameDatabaseContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Room>> GetAllAsync()
    {
        return await _context.Rooms.ToListAsync();
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _context.Rooms.FindAsync(id);
    }
}
