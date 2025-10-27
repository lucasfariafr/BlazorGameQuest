
namespace BlazorGame.GameService.Data;

public class GameDatabaseContext : DbContext
{

    public DbSet<Weapon> Weapons { get; set; } = null!;

    public DbSet<Potion> Potions { get; set; } = null!;
    
    public DbSet<Player> Player { get; set; } = null!;

    public DbSet<Monster> Monsters { get; set; } = null!;

    public DbSet<Chest> Chests { get; set; } = null!;

    public DbSet<Room> Rooms { get; set; } = null!;

    public DbSet<Dungeon> Dungeons { get; set; } = null!;

    public GameDatabaseContext(DbContextOptions<GameDatabaseContext> options) : base(options)
    {
        
    }
}