namespace BlazorGame.Tests.Tests;

/// <summary>
/// Tests unitaires pour vérifier le bon fonctionnement du DatabaseInitializer.
/// </summary>
public class DatabaseInitializerTests
{
    /// <summary>
    /// Crée un contexte en mémoire pour les tests.
    /// </summary>
    /// <returns>Contexte EF Core en mémoire.</returns>
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_DatabaseInitializerTest_{Guid.NewGuid()}")
            .Options;

        return new GameDatabaseContext(options);
    }

    /// <summary>
    /// Vérifie que la base de données est bien initialisée quand elle est vide.
    /// </summary>
    [Fact]
    public void Initialize_ShouldInitializeDatabase_WhenEmpty()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        Assert.Multiple(
            () => Assert.True(context.Weapons.Any(), "Weapons should be initialized."),
            () => Assert.True(context.Potions.Any(), "Potions should be initialized."),
            () => Assert.True(context.Player.Any(), "Players should be initialized."),
            () => Assert.True(context.Monsters.Any(), "Monsters should be initialized."),
            () => Assert.True(context.Chests.Any(), "Chests should be initialized."),
            () => Assert.True(context.Rooms.Any(), "Rooms should be initialized."),
            () => Assert.True(context.Dungeons.Any(), "Dungeons should be initialized.")
        );

        var dungeon = context.Dungeons.Include(d => d.Rooms).FirstOrDefault();
        Assert.NotNull(context.Dungeons.Include(d => d.Rooms).FirstOrDefault());
        Assert.Equal(DungeonLevel.Easy, dungeon!.DifficultyLevel);
        Assert.False(dungeon.IsCompleted);
        Assert.Equal(2, dungeon.Rooms.Count);
    }

    /// <summary>
    /// Vérifie que l'initialisation ne duplique pas les données si la base est déjà initialisée.
    /// </summary>
    [Fact]
    public void Initialize_ShouldNotDuplicateData_WhenAlreadyInitialized()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);
        DatabaseInitializer.Initialize(context);

        Assert.Multiple(
            () => Assert.Single(context.Dungeons),
            () => Assert.Equal(3, context.Weapons.Count()),
            () => Assert.Equal(2, context.Potions.Count()),
            () => Assert.Equal(1, context.Player.Count())
        );
    }
}
