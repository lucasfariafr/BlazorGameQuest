using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Tests.Tests.Controllers;

/// <summary>
/// Tests unitaires pour RoomsController.
/// </summary>
public class RoomsControllerTests
{
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_RoomsControllerTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    private static (GameDatabaseContext, RoomsController) CreateController()
    {
        var context = CreateInMemoryContext();
        var service = new RoomsService(context);
        var controller = new RoomsController(service);
        return (context, controller);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var room = context.Rooms.First();

        var result = await controller.GetById(room.RoomId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetById(9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
