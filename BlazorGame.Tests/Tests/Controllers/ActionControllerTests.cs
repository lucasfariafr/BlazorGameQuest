using BlazorGame.GameService.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.Tests.Tests.Controllers;

/// <summary>
/// Tests unitaires pour ActionController.
/// </summary>
public class ActionControllerTests
{
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_ActionControllerTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    private static (GameDatabaseContext, ActionController, GameSessionService) CreateController()
    {
        var context = CreateInMemoryContext();
        var playerService = new PlayerService(context);
        var monsterService = new MonstersService(context);
        var fightService = new FightService(playerService, monsterService, context);
        var dungeonsService = new DungeonsService(context);
        var sessionService = new GameSessionService(context, dungeonsService);
        var actionService = new ActionService(context, playerService, fightService, sessionService);
        var controller = new ActionController(actionService);
        return (context, controller, sessionService);
    }

    [Fact]
    public async Task Fight_WithValidParams_ShouldReturnOkResult()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;
        var room = session.Dungeon!.Rooms.FirstOrDefault(r => r.Monster != null);

        if (room != null)
        {
            var result = await controller.Fight(player.CharacterId, room.RoomId, session.DungeonId);

            Assert.IsType<OkObjectResult>(result);
        }
    }

    [Fact]
    public async Task Fight_WithInvalidPlayer_ShouldReturnOkWithError()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);

        var result = await controller.Fight(9999, 1, session.DungeonId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var actionResult = okResult.Value as ActionResultDto;
        Assert.NotNull(actionResult);
        Assert.False(actionResult.Success);
    }

    [Fact]
    public async Task RunAway_WithValidParams_ShouldReturnOkResult()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;
        var room = session.Dungeon!.Rooms.First();

        var result = await controller.RunAway(player.CharacterId, room.RoomId, session.DungeonId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task RunAway_WithInvalidPlayer_ShouldReturnOkWithError()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);

        var result = await controller.RunAway(9999, 1, session.DungeonId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var actionResult = okResult.Value as ActionResultDto;
        Assert.NotNull(actionResult);
        Assert.False(actionResult.Success);
    }

    [Fact]
    public async Task Search_WithValidParams_ShouldReturnOkResult()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;
        var room = session.Dungeon!.Rooms.First();

        var result = await controller.Search(player.CharacterId, room.RoomId, session.DungeonId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Search_WithInvalidPlayer_ShouldReturnOkWithError()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);

        var result = await controller.Search(9999, 1, session.DungeonId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var actionResult = okResult.Value as ActionResultDto;
        Assert.NotNull(actionResult);
        Assert.False(actionResult.Success);
    }

    [Fact]
    public async Task OpenChest_WithValidParams_ShouldReturnOkResult()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 10);
        var player = session.Player!;
        var room = session.Dungeon!.Rooms.FirstOrDefault(r => r.Chest != null);

        if (room != null)
        {
            var result = await controller.OpenChest(player.CharacterId, room.RoomId, session.DungeonId);

            Assert.IsType<OkObjectResult>(result);
        }
    }

    [Fact]
    public async Task OpenChest_WithInvalidPlayer_ShouldReturnOkWithError()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);

        var result = await controller.OpenChest(9999, 1, session.DungeonId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var actionResult = okResult.Value as ActionResultDto;
        Assert.NotNull(actionResult);
        Assert.False(actionResult.Success);
    }

    [Fact]
    public async Task Ignore_WithValidParams_ShouldReturnOkResult()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;
        var room = session.Dungeon!.Rooms.First();

        var result = await controller.Ignore(player.CharacterId, room.RoomId, session.DungeonId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Ignore_WithInvalidPlayer_ShouldReturnOkWithError()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);

        var result = await controller.Ignore(9999, 1, session.DungeonId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var actionResult = okResult.Value as ActionResultDto;
        Assert.NotNull(actionResult);
        Assert.False(actionResult.Success);
    }

    [Fact]
    public async Task Fight_WithInvalidRoom_ShouldReturnOkWithError()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;

        var result = await controller.Fight(player.CharacterId, 9999, session.DungeonId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var actionResult = okResult.Value as ActionResultDto;
        Assert.NotNull(actionResult);
        Assert.False(actionResult.Success);
    }

    [Fact]
    public async Task RunAway_WithInvalidRoom_ShouldReturnOkWithError()
    {
        var (context, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;

        var result = await controller.RunAway(player.CharacterId, 9999, session.DungeonId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var actionResult = okResult.Value as ActionResultDto;
        Assert.NotNull(actionResult);
        Assert.False(actionResult.Success);
    }
}
