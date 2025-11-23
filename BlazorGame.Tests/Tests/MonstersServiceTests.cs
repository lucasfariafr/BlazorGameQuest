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

    /// <summary>
    /// Vérifie que GetMonsterByIdAsync retourne null si le monstre n'existe pas.
    /// </summary>
    [Fact]
    public async Task GetMonsterByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var (_, service) = CreateService();

        var monster = await service.GetMonsterByIdAsync(999);

        Assert.Null(monster);
    }

    /// <summary>
    /// Vérifie que IsMonsterAliveAsync retourne true si le monstre est vivant.
    /// </summary>
    [Fact]
    public async Task IsMonsterAliveAsync_ShouldReturnTrue_WhenHealthIsPositive()
    {
        var (context, service) = CreateService();
        var monster = context.Monsters.First();
        monster.Health = 50;
        context.SaveChanges();

        var isAlive = await service.IsMonsterAliveAsync(monster.CharacterId);

        Assert.True(isAlive);
    }

    /// <summary>
    /// Vérifie que UpdateMonsterHealthAsync retourne false si le monstre n'existe pas.
    /// </summary>
    [Fact]
    public async Task UpdateMonsterHealthAsync_ShouldReturnFalse_WhenMonsterNotExists()
    {
        var (_, service) = CreateService();

        var result = await service.UpdateMonsterHealthAsync(999, 50);

        Assert.False(result);
    }

    /// <summary>
    /// Vérifie que GetMonstersByTypeAsync retourne une liste vide si aucun monstre du type.
    /// </summary>
    [Fact]
    public async Task GetMonstersByTypeAsync_ShouldReturnEmpty_WhenNoMonstersOfType()
    {
        var (context, service) = CreateService();
        // Supprimer tous les Gobelins
        var goblins = context.Monsters.Where(m => m.Type == MonsterType.Goblin).ToList();
        context.Monsters.RemoveRange(goblins);
        context.SaveChanges();

        var result = await service.GetMonstersByTypeAsync(MonsterType.Goblin);

        Assert.Empty(result);
    }

    /// <summary>
    /// Vérifie que les monstres ont des IDs uniques.
    /// </summary>
    [Fact]
    public async Task GetAllMonstersAsync_ShouldReturnMonstersWithUniqueIds()
    {
        var (_, service) = CreateService();

        var monsters = await service.GetAllMonstersAsync();
        var monsterIds = monsters.Select(m => m.CharacterId).ToList();

        Assert.Equal(monsterIds.Distinct().Count(), monsterIds.Count);
    }

    /// <summary>
    /// Vérifie que les monstres ont des stats positives.
    /// </summary>
    [Fact]
    public async Task GetAllMonstersAsync_Monsters_ShouldHavePositiveStats()
    {
        var (_, service) = CreateService();

        var monsters = await service.GetAllMonstersAsync();

        Assert.All(monsters, m =>
        {
            Assert.True(m.Strength > 0);
        });
    }

    /// <summary>
    /// Vérifie que GetMonsterByIdAsync retourne le monstre avec son arme.
    /// </summary>
    [Fact]
    public async Task GetMonsterByIdAsync_ShouldReturnMonster_WithWeapon()
    {
        var (context, service) = CreateService();
        var existingMonster = context.Monsters.Include(m => m.Weapon).First();

        var monster = await service.GetMonsterByIdAsync(existingMonster.CharacterId);

        Assert.NotNull(monster);
        Assert.NotNull(monster.Weapon);
    }

    /// <summary>
    /// Vérifie que UpdateMonsterHealthAsync modifie la santé correctement.
    /// </summary>
    [Fact]
    public async Task UpdateMonsterHealthAsync_ShouldSetCorrectHealth()
    {
        var (context, service) = CreateService();
        var monster = context.Monsters.First();
        var newHealth = 42.0;

        await service.UpdateMonsterHealthAsync(monster.CharacterId, newHealth);

        var updated = await context.Monsters.FindAsync(monster.CharacterId);
        Assert.NotNull(updated);
        Assert.Equal(newHealth, updated.Health);
    }

    /// <summary>
    /// Vérifie que IsMonsterAliveAsync retourne true pour un monstre avec santé positive.
    /// </summary>
    [Fact]
    public async Task IsMonsterAliveAsync_ShouldReturnTrue_WhenMonsterHasHealth()
    {
        var (context, service) = CreateService();
        var monster = context.Monsters.First();
        monster.Health = 100;
        context.SaveChanges();

        var isAlive = await service.IsMonsterAliveAsync(monster.CharacterId);

        Assert.True(isAlive);
    }

    /// <summary>
    /// Vérifie que IsMonsterAliveAsync retourne false pour un monstre inexistant.
    /// </summary>
    [Fact]
    public async Task IsMonsterAliveAsync_ShouldReturnFalse_WhenMonsterNotExists()
    {
        var (_, service) = CreateService();

        var isAlive = await service.IsMonsterAliveAsync(999);

        Assert.False(isAlive);
    }

    /// <summary>
    /// Vérifie que GetMonstersByTypeAsync retourne les Zombies correctement.
    /// </summary>
    [Fact]
    public async Task GetMonstersByTypeAsync_ShouldReturnZombies()
    {
        var (_, service) = CreateService();

        var zombies = await service.GetMonstersByTypeAsync(MonsterType.Zombie);

        Assert.All(zombies, z => Assert.Equal(MonsterType.Zombie, z.Type));
    }

    /// <summary>
    /// Vérifie que les monstres ont un type valide.
    /// </summary>
    [Fact]
    public async Task GetAllMonstersAsync_Monsters_ShouldHaveValidType()
    {
        var (_, service) = CreateService();

        var monsters = await service.GetAllMonstersAsync();

        Assert.All(monsters, m =>
        {
            Assert.True(Enum.IsDefined(typeof(MonsterType), m.Type));
        });
    }

    /// <summary>
    /// Vérifie que UpdateMonsterHealthAsync avec 0 marque le monstre comme mort.
    /// </summary>
    [Fact]
    public async Task UpdateMonsterHealthAsync_WithZero_ShouldMakeMonsterDead()
    {
        var (context, service) = CreateService();
        var monster = context.Monsters.First();

        await service.UpdateMonsterHealthAsync(monster.CharacterId, 0);

        var isAlive = await service.IsMonsterAliveAsync(monster.CharacterId);
        Assert.False(isAlive);
    }
}
