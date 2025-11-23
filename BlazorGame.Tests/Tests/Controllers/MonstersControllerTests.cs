using BlazorGame.GameService.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.Tests.Tests.Controllers;

/// <summary>
/// Tests unitaires pour MonstersController.
/// </summary>
public class MonstersControllerTests
{
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_MonstersControllerTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    private static (GameDatabaseContext, MonstersController) CreateController()
    {
        var context = CreateInMemoryContext();
        var service = new MonstersService(context);
        var controller = new MonstersController(service);
        return (context, controller);
    }

    [Fact]
    public async Task GetAllMonsters_ShouldReturnOkResult()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetAllMonsters();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetMonsterById_WithValidId_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var monster = context.Monsters.First();

        var result = await controller.GetMonsterById(monster.CharacterId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetMonsterById_WithInvalidId_ShouldReturnNotFound()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetMonsterById(9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetMonstersByType_Goblin_ShouldReturnOkResult()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetMonstersByType(MonsterType.Goblin);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetMonstersByType_Zombie_ShouldReturnOkResult()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetMonstersByType(MonsterType.Zombie);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task IsMonsterAlive_WithValidId_ShouldReturnOkResult()
    {
        var (context, controller) = CreateController();
        var monster = context.Monsters.First();

        var result = await controller.IsMonsterAlive(monster.CharacterId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task IsMonsterAlive_WithInvalidId_ShouldReturnNotFound()
    {
        var (_, controller) = CreateController();

        var result = await controller.IsMonsterAlive(9999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
