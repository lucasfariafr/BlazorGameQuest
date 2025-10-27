namespace BlazorGame.GameService.Data;

/// <summary>
/// Contexte de la base de données du jeu.
/// Contient toutes les entités : joueurs, monstres, armes, potions, salles, coffres et donjons.
/// </summary>
public class GameDatabaseContext : DbContext
{
    /// <summary>Table des armes.</summary>
    public DbSet<Weapon> Weapons { get; set; } = null!;

    /// <summary>Table des potions.</summary>
    public DbSet<Potion> Potions { get; set; } = null!;

    /// <summary>Table des joueurs.</summary>
    public DbSet<Player> Player { get; set; } = null!;

    /// <summary>Table des monstres.</summary>
    public DbSet<Monster> Monsters { get; set; } = null!;

    /// <summary>Table des coffres.</summary>
    public DbSet<Chest> Chests { get; set; } = null!;

    /// <summary>Table des salles.</summary>
    public DbSet<Room> Rooms { get; set; } = null!;

    /// <summary>Table des donjons.</summary>
    public DbSet<Dungeon> Dungeons { get; set; } = null!;

    /// <summary>
    /// Initialise le contexte avec les options spécifiées.
    /// </summary>
    /// <param name="options">Options du contexte.</param>
    public GameDatabaseContext(DbContextOptions<GameDatabaseContext> options) : base(options)
    {
    }
}
