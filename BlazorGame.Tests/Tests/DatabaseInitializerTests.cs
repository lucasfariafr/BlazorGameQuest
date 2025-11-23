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

    /// <summary>
    /// Vérifie que les armes sont créées avec les bonnes propriétés.
    /// </summary>
    [Fact]
    public void Initialize_ShouldCreateWeapons_WithCorrectProperties()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        var weapons = context.Weapons.ToList();
        Assert.Equal(3, weapons.Count);
        Assert.All(weapons, w => Assert.True(w.WeaponId > 0));
        Assert.All(weapons, w => Assert.True(Enum.IsDefined(typeof(WeaponType), w.Type)));
    }

    /// <summary>
    /// Vérifie que les potions sont créées avec les bonnes propriétés.
    /// </summary>
    [Fact]
    public void Initialize_ShouldCreatePotions_WithCorrectProperties()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        var potions = context.Potions.ToList();
        Assert.Equal(2, potions.Count);
        Assert.All(potions, p => Assert.True(p.PotionId > 0));
        Assert.All(potions, p => Assert.True(Enum.IsDefined(typeof(PotionType), p.Type)));
    }

    /// <summary>
    /// Vérifie que le joueur initial est créé avec les bonnes stats.
    /// </summary>
    [Fact]
    public void Initialize_ShouldCreatePlayer_WithCorrectStats()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        var player = context.Player.Include(p => p.Weapon).First();
        Assert.NotNull(player);
        Assert.True(player.Strength > 0);
        Assert.True(player.Health > 0);
        Assert.True(player.HeartNumber > 0);
    }

    /// <summary>
    /// Vérifie que les monstres sont créés avec leurs armes.
    /// </summary>
    [Fact]
    public void Initialize_ShouldCreateMonsters_WithWeapons()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        var monsters = context.Monsters.Include(m => m.Weapon).ToList();
        Assert.NotEmpty(monsters);
        Assert.All(monsters, m =>
        {
            Assert.NotNull(m.Weapon);
            Assert.True(m.Strength > 0);
        });
    }

    /// <summary>
    /// Vérifie que les coffres sont créés correctement.
    /// </summary>
    [Fact]
    public void Initialize_ShouldCreateChests_NotOpened()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        var chests = context.Chests.ToList();
        Assert.NotEmpty(chests);
        Assert.All(chests, c => Assert.False(c.IsOpened));
    }

    /// <summary>
    /// Vérifie que les salles ont des monstres ou des coffres.
    /// </summary>
    [Fact]
    public void Initialize_ShouldCreateRooms_WithMonstersOrChests()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        var rooms = context.Rooms.Include(r => r.Monster).Include(r => r.Chest).ToList();
        Assert.Equal(2, rooms.Count);
        Assert.All(rooms, r => Assert.True(r.Monster != null || r.Chest != null));
    }

    /// <summary>
    /// Vérifie que les salles ont des descriptions.
    /// </summary>
    [Fact]
    public void Initialize_ShouldCreateRooms_WithDescriptions()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        var rooms = context.Rooms.ToList();
        Assert.All(rooms, r => Assert.False(string.IsNullOrWhiteSpace(r.Description)));
    }

    /// <summary>
    /// Vérifie que les salles ont des actions disponibles.
    /// </summary>
    [Fact]
    public void Initialize_ShouldCreateRooms_WithActions()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        var rooms = context.Rooms.ToList();
        Assert.All(rooms, r =>
        {
            Assert.NotNull(r.Actions);
            Assert.NotEmpty(r.Actions);
        });
    }

    /// <summary>
    /// Vérifie que le donjon initial a le bon niveau de difficulté.
    /// </summary>
    [Fact]
    public void Initialize_ShouldCreateDungeon_WithEasyLevel()
    {
        using var context = CreateInMemoryContext();

        DatabaseInitializer.Initialize(context);

        var dungeon = context.Dungeons.First();
        Assert.Equal(DungeonLevel.Easy, dungeon.DifficultyLevel);
    }
}
