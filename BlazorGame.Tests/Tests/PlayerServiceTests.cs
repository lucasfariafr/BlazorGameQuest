namespace BlazorGame.Tests.Tests;

/// <summary>
/// Tests unitaires pour vérifier le bon fonctionnement du PlayerService.
/// </summary>
public class PlayerServiceTests
{
    /// <summary>
    /// Crée un contexte EF Core en mémoire et initialise la base de données.
    /// </summary>
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_PlayerTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    /// <summary>
    /// Prépare le service PlayerService avec un contexte en mémoire.
    /// </summary>
    private static (GameDatabaseContext Context, PlayerService Service) CreateService()
    {
        var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        return (context, service);
    }

    /// <summary>
    /// Vérifie que GetPlayerByIdAsync retourne le joueur correct s’il existe.
    /// </summary>
    [Fact]
    public async Task GetPlayerByIdAsync_ShouldReturnPlayer_WhenExists()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var existingPlayer = context.Player.First();

        var foundPlayer = await service.GetPlayerByIdAsync(existingPlayer.CharacterId);

        Assert.NotNull(foundPlayer);
        Assert.Equal(existingPlayer.CharacterId, foundPlayer.CharacterId);
    }

    /// <summary>
    /// Vérifie que UpdatePlayerHealthAsync met correctement à jour la santé du joueur.
    /// </summary>
    [Fact]
    public async Task UpdatePlayerHealthAsync_ShouldUpdateHealthValue()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();
        var newHealth = 50;

        await service.UpdatePlayerHealthAsync(player.CharacterId, newHealth);

        var updated = await context.Player.FindAsync(player.CharacterId);
        Assert.NotNull(updated);
        Assert.Equal(newHealth, updated.Health);
    }

    /// <summary>
    /// Vérifie que UsePotionAsync augmente la santé du joueur et supprime la potion utilisée.
    /// </summary>
    [Fact]
    public async Task UsePotionAsync_ShouldIncreaseHealthAndRemovePotion()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);

        var player = context.Player.Include(p => p.Potions).First();
        player.Health = 50;

        var potion = new Potion
        {
            PotionId = 50,
            Type = PotionType.Health
        };

        context.Potions.Add(potion);
        await context.SaveChangesAsync();

        player.Potions ??= new List<Potion>();
        player.Potions.Add(potion);
        await context.SaveChangesAsync();

        var updatedPlayer = await service.UsePotionAsync(player.CharacterId, potion.PotionId);

        Assert.NotNull(updatedPlayer);
        Assert.True(updatedPlayer.Health > 50, "La santé du joueur doit avoir augmenté.");
        Assert.DoesNotContain(updatedPlayer.Potions!, p => p.PotionId == potion.PotionId);
        Assert.InRange(updatedPlayer.Health, 51, GameConstants.MaxHealth);
    }
}
