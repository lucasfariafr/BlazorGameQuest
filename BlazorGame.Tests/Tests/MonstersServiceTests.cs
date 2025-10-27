namespace BlazorGame.Tests.Tests;

/// <summary>
/// Tests unitaires pour vérifier le bon fonctionnement du MonstersService.
/// </summary>
public class MonstersServiceTests
{
    /// <summary>
    /// Crée un contexte EF Core en mémoire et initialise la base de données.
    /// </summary>
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_MonstersTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    /// <summary>
    /// Prépare le service MonstersService avec un contexte en mémoire.
    /// </summary>
    private static (GameDatabaseContext Context, MonstersService Service) CreateService()
    {
        var context = CreateInMemoryContext();
        var service = new MonstersService(context);
        return (context, service);
    }

    /// <summary>
    /// Vérifie que GetAllMonstersAsync retourne tous les monstres.
    /// </summary>
    [Fact]
    public async Task GetAllMonstersAsync_ShouldReturnAllMonsters()
    {
        var (context, service) = CreateService();

        var monsters = await service.GetAllMonstersAsync();

        Assert.NotEmpty(monsters);
        Assert.All(monsters, m => Assert.NotNull(m.Weapon));
    }

    /// <summary>
    /// Vérifie que GetMonsterByIdAsync retourne le monstre correct si existant.
    /// </summary>
    [Fact]
    public async Task GetMonsterByIdAsync_ShouldReturnMonster_WhenExists()
    {
        var (context, service) = CreateService();
        var existingMonster = context.Monsters.First();

        var monster = await service.GetMonsterByIdAsync(existingMonster.CharacterId);

        Assert.NotNull(monster);
        Assert.Equal(existingMonster.CharacterId, monster.CharacterId);
    }

    /// <summary>
    /// Vérifie que GetMonstersByTypeAsync filtre correctement les monstres par type.
    /// </summary>
    [Fact]
    public async Task GetMonstersByTypeAsync_ShouldFilterByType()
    {
        var (context, service) = CreateService();

        var zombies = await service.GetMonstersByTypeAsync(MonsterType.Zombie);

        Assert.NotEmpty(zombies);
        Assert.All(zombies, z => Assert.Equal(MonsterType.Zombie, z.Type));
    }

    /// <summary>
    /// Vérifie que UpdateMonsterHealthAsync modifie correctement la santé du monstre.
    /// </summary>
    [Fact]
    public async Task UpdateMonsterHealthAsync_ShouldModifyHealthValue()
    {
        var (context, service) = CreateService();
        var monster = context.Monsters.First();

        var isUpdated = await service.UpdateMonsterHealthAsync(monster.CharacterId, 5);

        Assert.True(isUpdated);
        var updated = await context.Monsters.FindAsync(monster.CharacterId);
        Assert.Equal(5, updated!.Health);
    }

    /// <summary>
    /// Vérifie que IsMonsterAliveAsync retourne false lorsque la santé du monstre est zéro.
    /// </summary>
    [Fact]
    public async Task IsMonsterAliveAsync_ShouldReturnFalse_WhenHealthIsZero()
    {
        var (context, service) = CreateService();
        var monster = context.Monsters.First();
        monster.Health = 0;
        context.SaveChanges();

        var isAlive = await service.IsMonsterAliveAsync(monster.CharacterId);

        Assert.False(isAlive);
    }
}
