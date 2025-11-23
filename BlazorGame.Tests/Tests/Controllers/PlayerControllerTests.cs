using BlazorGame.GameService.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.Tests.Tests.Controllers;

/// <summary>
/// Tests unitaires pour PlayerController.
/// </summary>
public class PlayerControllerTests
{
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_PlayerControllerTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    private static (GameDatabaseContext, PlayerController) CreateController()
    {
        var context = CreateInMemoryContext();
        var service = new PlayerService(context);
        var controller = new PlayerController(service);
        return (context, controller);
    }

    [Fact]
    public async Task GetAllPlayers_ShouldReturnOkResult()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetAllPlayers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetPlayer_WithValidId_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();

        var result = await controller.GetPlayer(player.CharacterId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPlayer_WithInvalidId_ShouldReturnNotFound()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetPlayer(9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task IsPlayerAlive_WithValidId_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();

        var result = await controller.IsPlayerAlive(player.CharacterId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task IsPlayerAlive_WithInvalidId_ShouldReturnNotFound()
    {
        var (_, controller) = CreateController();

        var result = await controller.IsPlayerAlive(9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task EquipWeapon_WithValidIds_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();
        var weapon = context.Weapons.First();

        var result = await controller.EquipWeapon(player.CharacterId, weapon.WeaponId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task EquipWeapon_WithInvalidPlayerId_ShouldReturnNotFound()
    {
        var (context, controller) = CreateController();
        var weapon = context.Weapons.First();

        var result = await controller.EquipWeapon(9999, weapon.WeaponId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task EquipWeapon_WithInvalidWeaponId_ShouldReturnNotFound()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();

        var result = await controller.EquipWeapon(player.CharacterId, 9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AddPotion_WithValidData_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();
        var potion = new Potion { PotionId = 100, Type = PotionType.Health };
        context.Potions.Add(potion);
        await context.SaveChangesAsync();

        var result = await controller.AddPotion(player.CharacterId, potion);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddPotion_WithInvalidPlayerId_ShouldReturnNotFound()
    {
        var (context, controller) = CreateController();
        var potion = new Potion { PotionId = 101, Type = PotionType.Health };
        context.Potions.Add(potion);
        await context.SaveChangesAsync();

        var result = await controller.AddPotion(9999, potion);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AddPotion_WithNullPotion_ShouldReturnBadRequest()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();

        var result = await controller.AddPotion(player.CharacterId, null!);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UsePotion_WithInvalidPotion_ShouldReturnBadRequest()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();

        var result = await controller.UsePotion(player.CharacterId, 9999);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
