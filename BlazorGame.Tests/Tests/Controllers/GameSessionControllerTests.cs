using BlazorGame.GameService.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.Tests.Tests.Controllers;

/// <summary>
/// Tests unitaires pour GameSessionController.
/// </summary>
public class GameSessionControllerTests
{
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_SessionControllerTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    private static (GameDatabaseContext, GameSessionController, GameSessionService) CreateController()
    {
        var context = CreateInMemoryContext();
        var dungeonsService = new DungeonsService(context);
        var sessionService = new GameSessionService(context, dungeonsService);
        var playerService = new PlayerService(context);
        var controller = new GameSessionController(sessionService, playerService);
        return (context, controller, sessionService);
    }

    [Fact]
    public async Task StartNewGame_WithDefaultParams_ShouldReturnCreated()
    {
        var (_, controller, _) = CreateController();

        var result = await controller.StartNewGame(null);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task StartNewGame_WithEasyLevel_ShouldReturnCreated()
    {
        var (_, controller, _) = CreateController();
        var request = new StartGameRequestDto { DifficultyLevel = "easy", RoomCount = 5 };

        var result = await controller.StartNewGame(request);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task StartNewGame_WithMediumLevel_ShouldReturnCreated()
    {
        var (_, controller, _) = CreateController();
        var request = new StartGameRequestDto { DifficultyLevel = "medium", RoomCount = 7 };

        var result = await controller.StartNewGame(request);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task StartNewGame_WithHardLevel_ShouldReturnCreated()
    {
        var (_, controller, _) = CreateController();
        var request = new StartGameRequestDto { DifficultyLevel = "hard", RoomCount = 10 };

        var result = await controller.StartNewGame(request);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task GetSession_WithValidId_ShouldReturnOkResult()
    {
        var (_, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);

        var result = await controller.GetSession(session.SessionId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetSession_WithInvalidId_ShouldReturnNotFound()
    {
        var (_, controller, _) = CreateController();

        var result = await controller.GetSession(9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetScoreHistory_ShouldReturnOkResult()
    {
        var (_, controller, _) = CreateController();

        var result = await controller.GetScoreHistory();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetCompletedSessions_ShouldReturnOkResult()
    {
        var (_, controller, _) = CreateController();

        var result = await controller.GetCompletedSessions();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AbandonGame_WithValidId_ShouldReturnOkResult()
    {
        var (_, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);

        var result = await controller.AbandonGame(session.SessionId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AbandonGame_WithInvalidId_ShouldReturnNotFound()
    {
        var (_, controller, _) = CreateController();

        var result = await controller.AbandonGame(9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }



    [Fact]
    public async Task SaveGame_WithInvalidId_ShouldReturnNotFound()
    {
        var (_, controller, _) = CreateController();

        var result = await controller.SaveGame(9999, 1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetSavedGames_ShouldReturnOkResult()
    {
        var (_, controller, _) = CreateController();

        var result = await controller.GetSavedGames();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ResumeGame_WithSavedSession_ShouldReturnOkResult()
    {
        var (_, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        await sessionService.SaveGameAsync(session.SessionId, session.CurrentRoomId);

        var result = await controller.ResumeGame(session.SessionId);

        Assert.IsType<OkObjectResult>(result);
    }



    [Fact]
    public async Task DeleteSavedGame_WithSavedSession_ShouldReturnOkResult()
    {
        var (_, controller, sessionService) = CreateController();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        await sessionService.SaveGameAsync(session.SessionId, session.CurrentRoomId);

        var result = await controller.DeleteSavedGame(session.SessionId);

        Assert.IsType<OkObjectResult>(result);
    }

}
