namespace BlazorGame.Tests.Tests;

/// <summary>
/// Tests unitaires pour vérifier le bon fonctionnement du FightService.
/// </summary>
public class FightServiceTests
{
    /// <summary>
    /// Crée un contexte en mémoire et initialise la base de données.
    /// </summary>
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_FightTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    /// <summary>
    /// Prépare le service de combat et retourne un joueur et un monstre prêts pour le test.
    /// </summary>
    private static (FightService Service, Player Player, Monster Monster) CreateFightSetup(GameDatabaseContext context)
    {
        var playerService = new PlayerService(context);
        var monsterService = new MonstersService(context);
        var fightService = new FightService(playerService, monsterService, context);

        var player = context.Player.First();
        var monster = context.Monsters.First();

        player.Health = 100;
        monster.Health = 50;
        context.SaveChanges();

        return (fightService, player, monster);
    }

    /// <summary>
    /// Vérifie que le joueur gagne lorsque sa force est supérieure.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldReturnVictory_WhenPlayerStronger()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 40;
        monster.Strength = 5;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.NotNull(result);
        Assert.Contains("Victoire", result.Result);
    }

    /// <summary>
    /// Vérifie que le joueur perd lorsque le monstre est plus fort.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldReturnDefeat_WhenMonsterStronger()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 5;
        monster.Strength = 40;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.Contains("Défaite", result.Result);
    }

    /// <summary>
    /// Vérifie que l’exception est levée lorsque le joueur n’existe pas.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldThrow_WhenPlayerNotFound()
    {
        using var context = CreateInMemoryContext();
        var (service, _, monster) = CreateFightSetup(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.ExecuteFightAsync(999, monster.CharacterId));
    }
}
