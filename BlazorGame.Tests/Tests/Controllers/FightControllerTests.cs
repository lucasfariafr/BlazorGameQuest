using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Tests.Tests.Controllers;

/// <summary>
/// Tests unitaires pour FightController.
/// </summary>
public class FightControllerTests
{
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_FightControllerTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    private static (GameDatabaseContext, FightController) CreateController()
    {
        var context = CreateInMemoryContext();
        var playerService = new PlayerService(context);
        var monsterService = new MonstersService(context);
        var fightService = new FightService(playerService, monsterService, context);
        var controller = new FightController(fightService, playerService, monsterService);
        return (context, controller);
    }

    [Fact]
    public async Task Fight_WithInvalidPlayerId_ShouldReturnNotFound()
    {
        var (context, controller) = CreateController();
        var monster = context.Monsters.First();

        var result = await controller.Fight(9999, monster.CharacterId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Fight_WithInvalidMonsterId_ShouldReturnNotFound()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();

        var result = await controller.Fight(player.CharacterId, 9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Fight_WithValidIds_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();
        var monster = context.Monsters.First();

        var result = await controller.Fight(player.CharacterId, monster.CharacterId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task SimulateFight_WithValidIds_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();
        var monster = context.Monsters.First();

        var result = await controller.SimulateFight(player.CharacterId, monster.CharacterId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task SimulateFight_WithInvalidPlayerId_ShouldReturnNotFound()
    {
        var (context, controller) = CreateController();
        var monster = context.Monsters.First();

        var result = await controller.SimulateFight(9999, monster.CharacterId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task SimulateFight_WithInvalidMonsterId_ShouldReturnNotFound()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();

        var result = await controller.SimulateFight(player.CharacterId, 9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Fight_WithDeadPlayer_ShouldReturnBadRequest()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();
        player.Health = 0;
        player.HeartNumber = 0;
        context.SaveChanges();
        var monster = context.Monsters.First();

        var result = await controller.Fight(player.CharacterId, monster.CharacterId);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Fight_WithDeadMonster_ShouldReturnBadRequest()
    {
        var (context, controller) = CreateController();
        var player = context.Player.First();
        var monster = context.Monsters.First();
        monster.Health = 0;
        context.SaveChanges();

        var result = await controller.Fight(player.CharacterId, monster.CharacterId);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
