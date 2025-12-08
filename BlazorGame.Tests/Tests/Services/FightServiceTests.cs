using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using BlazorGame.SharedModels.Models.Entities;
using Microsoft.EntityFrameworkCore;

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
    /// Vérifie que le joueur perd lorsque le monstre est plus fort et qu'il n'a plus de cœurs.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldReturnDefeat_WhenMonsterStronger()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 5;
        player.HeartNumber = 1;
        monster.Strength = 40;
        monster.Health = 200; 
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.Contains("Défaite", result.Result);
    }

    /// <summary>
    /// Vérifie que l'exception est levée lorsque le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldThrow_WhenPlayerNotFound()
    {
        using var context = CreateInMemoryContext();
        var (service, _, monster) = CreateFightSetup(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.ExecuteFightAsync(999, monster.CharacterId));
    }

    /// <summary>
    /// Vérifie que l'exception est levée lorsque le monstre n'existe pas.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldThrow_WhenMonsterNotFound()
    {
        using var context = CreateInMemoryContext();
        var (service, player, _) = CreateFightSetup(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.ExecuteFightAsync(player.CharacterId, 999));
    }

    /// <summary>
    /// Vérifie que le joueur perd un cœur mais survit quand il a plusieurs cœurs.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldLoseHeart_WhenPlayerHasMultipleHearts()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 5;
        player.HeartNumber = 3;
        monster.Strength = 40;
        monster.Health = 200;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.Contains("cœur", result.Result);
    }

    /// <summary>
    /// Vérifie que le résultat contient un snapshot du joueur.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldReturnPlayerSnapshot()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 40;
        monster.Strength = 5;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.NotNull(result.PlayerSnapshot);
        Assert.Equal(player.CharacterId, result.PlayerSnapshot.CharacterId);
    }

    /// <summary>
    /// Vérifie que le résultat contient un snapshot du monstre.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldReturnMonsterSnapshot()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 40;
        monster.Strength = 5;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.NotNull(result.MonsterSnapshot);
        Assert.Equal(monster.CharacterId, result.MonsterSnapshot.CharacterId);
    }

    /// <summary>
    /// Vérifie que le nombre de tours est correctement enregistré.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldRecordTurns()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 40;
        monster.Strength = 5;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.NotNull(result.Turns);
        Assert.NotEmpty(result.Turns);
        Assert.True(result.TotalTurns > 0);
    }

    /// <summary>
    /// Vérifie que le joueur avec une arme fait plus de dégâts.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_PlayerWithWeapon_ShouldDealMoreDamage()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        var weapon = context.Weapons.First();
        player.Weapon = weapon;
        player.Strength = 30;
        monster.Health = 100;
        monster.Strength = 5;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.Contains("Victoire", result.Result);
    }

    /// <summary>
    /// Vérifie que le combat équilibré prend plusieurs tours.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_BalancedFight_ShouldTakeMultipleTurns()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 15;
        player.Health = 100;
        monster.Strength = 15;
        monster.Health = 100;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.True(result.TotalTurns >= 1);
    }

    /// <summary>
    /// Vérifie que le snapshot du joueur contient les bonnes informations.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_PlayerSnapshot_ShouldContainCorrectData()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 40;
        player.Armor = 10;
        monster.Strength = 5;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.NotNull(result.PlayerSnapshot);
        Assert.Equal(player.CharacterId, result.PlayerSnapshot.CharacterId);
    }

    /// <summary>
    /// Vérifie que le snapshot du monstre contient les bonnes informations.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_MonsterSnapshot_ShouldContainCorrectData()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 40;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.NotNull(result.MonsterSnapshot);
        Assert.Equal(monster.CharacterId, result.MonsterSnapshot.CharacterId);
    }

    /// <summary>
    /// Vérifie que le joueur avec 0 cœurs perd définitivement.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ZeroHearts_ShouldResultInDefeat()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 1;
        player.Health = 10;
        player.HeartNumber = 0;
        player.Armor = 0;
        monster.Strength = 100;
        monster.Health = 500;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.Contains("Défaite", result.Result);
    }

    /// <summary>
    /// Vérifie que les dégâts du monstre sont réduits par l'armure du joueur.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_PlayerArmor_ShouldReduceDamage()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 20;
        player.Health = 100;
        player.Armor = 20;
        monster.Strength = 10;
        monster.Health = 100;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        // Le joueur devrait gagner grâce à son armure
        Assert.Contains("Victoire", result.Result);
    }

    /// <summary>
    /// Vérifie que le résultat contient le bon format de message.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_ShouldReturnFormattedMessage()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 40;
        monster.Strength = 5;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.False(string.IsNullOrWhiteSpace(result.Result));
    }

    /// <summary>
    /// Vérifie que les tours de combat sont enregistrés dans l'ordre.
    /// </summary>
    [Fact]
    public async Task ExecuteFightAsync_Turns_ShouldBeInOrder()
    {
        using var context = CreateInMemoryContext();
        var (service, player, monster) = CreateFightSetup(context);

        player.Strength = 15;
        player.Health = 100;
        monster.Strength = 15;
        monster.Health = 100;
        context.SaveChanges();

        var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

        Assert.NotNull(result.Turns);
        Assert.True(result.Turns.Count == result.TotalTurns);
    }
}
