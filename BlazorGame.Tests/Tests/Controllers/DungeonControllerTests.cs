using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using BlazorGame.SharedModels.Enums.Environment;
using BlazorGame.SharedModels.Models.Environment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Tests.Tests.Controllers;

/// <summary>
/// Tests unitaires pour DungeonController.
/// </summary>
public class DungeonControllerTests
{
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_DungeonControllerTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    private static (GameDatabaseContext, DungeonController) CreateController()
    {
        var context = CreateInMemoryContext();
        var service = new DungeonsService(context);
        var controller = new DungeonController(service);
        return (context, controller);
    }

    [Fact]
    public async Task GetAllDungeons_ShouldReturnOkResult()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetAllDungeons();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetDungeonById_WithValidId_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var dungeon = context.Dungeons.First();

        var result = await controller.GetDungeonById(dungeon.DungeonId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetDungeonById_WithInvalidId_ShouldReturnNotFound()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetDungeonById(9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GenerateRandomDungeon_WithValidParams_ShouldReturnCreated()
    {
        var (_, controller) = CreateController();

        var result = await controller.GenerateRandomDungeon(DungeonLevel.Easy, 5);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public async Task GenerateRandomDungeon_WithInvalidRoomCount_ShouldReturnBadRequest()
    {
        var (_, controller) = CreateController();

        var result = await controller.GenerateRandomDungeon(DungeonLevel.Easy, 25);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GenerateRandomDungeon_WithZeroRooms_ShouldReturnBadRequest()
    {
        var (_, controller) = CreateController();

        var result = await controller.GenerateRandomDungeon(DungeonLevel.Easy, 0);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GenerateRandomDungeon_MediumLevel_ShouldReturnCreated()
    {
        var (_, controller) = CreateController();

        var result = await controller.GenerateRandomDungeon(DungeonLevel.Medium, 5);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task GenerateRandomDungeon_DifficultLevel_ShouldReturnCreated()
    {
        var (_, controller) = CreateController();

        var result = await controller.GenerateRandomDungeon(DungeonLevel.Difficult, 5);

        Assert.IsType<CreatedAtActionResult>(result);
    }
}
