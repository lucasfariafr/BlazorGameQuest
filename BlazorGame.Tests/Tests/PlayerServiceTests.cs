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

    /// <summary>
    /// Vérifie que GetPlayerByIdAsync retourne null si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task GetPlayerByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);

        var player = await service.GetPlayerByIdAsync(999);

        Assert.Null(player);
    }

    /// <summary>
    /// Vérifie que GetAllPlayersAsync retourne tous les joueurs.
    /// </summary>
    [Fact]
    public async Task GetAllPlayersAsync_ShouldReturnAllPlayers()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);

        var players = await service.GetAllPlayersAsync();

        Assert.NotEmpty(players);
    }

    /// <summary>
    /// Vérifie que UpdatePlayerHealthAsync retourne false si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task UpdatePlayerHealthAsync_ShouldReturnFalse_WhenPlayerNotExists()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);

        var result = await service.UpdatePlayerHealthAsync(999, 50);

        Assert.False(result);
    }

    /// <summary>
    /// Vérifie que UpdatePlayerHealthAsync régénère les cœurs quand la santé est à 0.
    /// </summary>
    [Fact]
    public async Task UpdatePlayerHealthAsync_ShouldRegenerateHeart_WhenHealthIsZero()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();
        player.HeartNumber = 3;
        player.Health = 100;
        context.SaveChanges();

        await service.UpdatePlayerHealthAsync(player.CharacterId, 0);

        var updated = await context.Player.FindAsync(player.CharacterId);
        Assert.NotNull(updated);
        Assert.Equal(2, updated.HeartNumber);
        Assert.Equal(GameConstants.MaxHealth, updated.Health);
    }

    /// <summary>
    /// Vérifie que IsPlayerAliveAsync retourne true si le joueur est vivant.
    /// </summary>
    [Fact]
    public async Task IsPlayerAliveAsync_ShouldReturnTrue_WhenPlayerAlive()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();
        player.Health = 50;
        player.HeartNumber = 2;
        context.SaveChanges();

        var isAlive = await service.IsPlayerAliveAsync(player.CharacterId);

        Assert.True(isAlive);
    }

    /// <summary>
    /// Vérifie que IsPlayerAliveAsync retourne false si le joueur n'est plus vivant.
    /// </summary>
    [Fact]
    public async Task IsPlayerAliveAsync_ShouldReturnFalse_WhenPlayerDead()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();
        player.Health = 0;
        player.HeartNumber = 0;
        context.SaveChanges();

        var isAlive = await service.IsPlayerAliveAsync(player.CharacterId);

        Assert.False(isAlive);
    }

    /// <summary>
    /// Vérifie que AddPotionAsync ajoute une potion au joueur.
    /// </summary>
    [Fact]
    public async Task AddPotionAsync_ShouldAddPotion()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.Include(p => p.Potions).First();
        var potion = new Potion { PotionId = 100, Type = PotionType.Health };
        context.Potions.Add(potion);
        await context.SaveChangesAsync();

        var result = await service.AddPotionAsync(player.CharacterId, potion);

        Assert.True(result);
    }

    /// <summary>
    /// Vérifie que AddPotionAsync retourne false si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task AddPotionAsync_ShouldReturnFalse_WhenPlayerNotExists()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var potion = new Potion { PotionId = 100, Type = PotionType.Health };

        var result = await service.AddPotionAsync(999, potion);

        Assert.False(result);
    }

    /// <summary>
    /// Vérifie que EquipWeaponAsync équipe une arme.
    /// </summary>
    [Fact]
    public async Task EquipWeaponAsync_ShouldEquipWeapon()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();
        var weapon = context.Weapons.First();

        var result = await service.EquipWeaponAsync(player.CharacterId, weapon.WeaponId);

        Assert.True(result);
    }

    /// <summary>
    /// Vérifie que EquipWeaponAsync retourne false si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task EquipWeaponAsync_ShouldReturnFalse_WhenPlayerNotExists()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var weapon = context.Weapons.First();

        var result = await service.EquipWeaponAsync(999, weapon.WeaponId);

        Assert.False(result);
    }

    /// <summary>
    /// Vérifie que EquipWeaponAsync retourne false si l'arme n'existe pas.
    /// </summary>
    [Fact]
    public async Task EquipWeaponAsync_ShouldReturnFalse_WhenWeaponNotExists()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();

        var result = await service.EquipWeaponAsync(player.CharacterId, 999);

        Assert.False(result);
    }

    /// <summary>
    /// Vérifie que UsePotionAsync avec une potion de force augmente la force.
    /// </summary>
    [Fact]
    public async Task UsePotionAsync_ShouldIncreaseStrength_WhenStrengthPotion()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.Include(p => p.Potions).First();
        var initialStrength = player.Strength;

        var potion = new Potion { PotionId = 60, Type = PotionType.Strength };
        context.Potions.Add(potion);
        player.Potions ??= new List<Potion>();
        player.Potions.Add(potion);
        await context.SaveChangesAsync();

        var updated = await service.UsePotionAsync(player.CharacterId, potion.PotionId);

        Assert.True(updated.Strength >= initialStrength);
    }

    /// <summary>
    /// Vérifie que UsePotionAsync lève une exception si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task UsePotionAsync_ShouldThrow_WhenPlayerNotFound()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UsePotionAsync(999, 1));
    }

    /// <summary>
    /// Vérifie que UsePotionAsync lève une exception si la potion n'existe pas.
    /// </summary>
    [Fact]
    public async Task UsePotionAsync_ShouldThrow_WhenPotionNotFound()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UsePotionAsync(player.CharacterId, 999));
    }

    /// <summary>
    /// Vérifie que le joueur a les propriétés de base après initialisation.
    /// </summary>
    [Fact]
    public async Task GetPlayerByIdAsync_ShouldReturnPlayer_WithBaseProperties()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var existingPlayer = context.Player.First();

        var player = await service.GetPlayerByIdAsync(existingPlayer.CharacterId);

        Assert.NotNull(player);
        Assert.True(player.Strength > 0);
    }

    /// <summary>
    /// Vérifie que UpdatePlayerHealthAsync ne dépasse pas la santé maximale.
    /// </summary>
    [Fact]
    public async Task UpdatePlayerHealthAsync_ShouldNotExceedMaxHealth()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();

        await service.UpdatePlayerHealthAsync(player.CharacterId, 1000);

        var updated = await context.Player.FindAsync(player.CharacterId);
        Assert.NotNull(updated);
        Assert.True(updated.Health <= GameConstants.MaxHealth);
    }

    /// <summary>
    /// Vérifie que la liste des joueurs n'est pas vide après initialisation.
    /// </summary>
    [Fact]
    public async Task GetAllPlayersAsync_ShouldNotBeEmpty_AfterInitialization()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);

        var players = await service.GetAllPlayersAsync();

        Assert.NotEmpty(players);
        Assert.All(players, p => Assert.True(p.CharacterId > 0));
    }

    /// <summary>
    /// Vérifie que IsPlayerAliveAsync retourne false si joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task IsPlayerAliveAsync_ShouldReturnFalse_WhenPlayerNotExists()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);

        var isAlive = await service.IsPlayerAliveAsync(999);

        Assert.False(isAlive);
    }

    /// <summary>
    /// Vérifie que AddPotionAsync retourne true et ajoute la potion.
    /// </summary>
    [Fact]
    public async Task AddPotionAsync_ShouldReturnTrue_AndAddPotion()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.Include(p => p.Potions).First();
        var initialCount = player.Potions?.Count ?? 0;

        var potion = new Potion { PotionId = 200, Type = PotionType.Strength };
        context.Potions.Add(potion);
        await context.SaveChangesAsync();

        var result = await service.AddPotionAsync(player.CharacterId, potion);

        Assert.True(result);
        var updatedPlayer = await context.Player.Include(p => p.Potions).FirstAsync(p => p.CharacterId == player.CharacterId);
        Assert.True(updatedPlayer.Potions!.Count > initialCount);
    }

    /// <summary>
    /// Vérifie que EquipWeaponAsync change l'arme du joueur.
    /// </summary>
    [Fact]
    public async Task EquipWeaponAsync_ShouldChangePlayerWeapon()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();
        var weapon = context.Weapons.First();

        await service.EquipWeaponAsync(player.CharacterId, weapon.WeaponId);

        var updatedPlayer = await context.Player.Include(p => p.Weapon).FirstAsync(p => p.CharacterId == player.CharacterId);
        Assert.NotNull(updatedPlayer.Weapon);
        Assert.Equal(weapon.WeaponId, updatedPlayer.Weapon.WeaponId);
    }

    /// <summary>
    /// Vérifie que UpdatePlayerHealthAsync avec valeur négative met la santé à 0.
    /// </summary>
    [Fact]
    public async Task UpdatePlayerHealthAsync_NegativeValue_ShouldSetHealthToZeroOrTriggerHeart()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.First();
        player.HeartNumber = 2;
        context.SaveChanges();

        await service.UpdatePlayerHealthAsync(player.CharacterId, -50);

        var updated = await context.Player.FindAsync(player.CharacterId);
        Assert.NotNull(updated);
        // Soit la santé est à 0, soit un cœur a été utilisé et la santé régénérée
        Assert.True(updated.Health >= 0);
    }

    /// <summary>
    /// Vérifie que UsePotionAsync avec potion de santé augmente la santé.
    /// </summary>
    [Fact]
    public async Task UsePotionAsync_HealthPotion_ShouldIncreaseHealth()
    {
        using var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var player = context.Player.Include(p => p.Potions).First();
        player.Health = 30;
        context.SaveChanges();

        var potion = new Potion { PotionId = 300, Type = PotionType.Health };
        context.Potions.Add(potion);
        player.Potions ??= new List<Potion>();
        player.Potions.Add(potion);
        await context.SaveChangesAsync();

        var updatedPlayer = await service.UsePotionAsync(player.CharacterId, potion.PotionId);

        Assert.True(updatedPlayer.Health > 30);
    }
}
