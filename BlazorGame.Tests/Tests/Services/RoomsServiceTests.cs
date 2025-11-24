using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Tests.Tests;

/// <summary>
/// Tests unitaires pour vérifier le bon fonctionnement du RoomsService.
/// </summary>
public class RoomsServiceTests
{
    /// <summary>
    /// Crée un contexte EF Core en mémoire et initialise la base de données.
    /// </summary>
    private GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase("GameDb_RoomsTest_" + Guid.NewGuid())
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    /// <summary>
    /// Vérifie que toutes les salles retournées par <see cref="RoomsService.GetAllRoomsAsync"/> 
    /// ne sont pas nulles et possèdent des propriétés valides.
    /// </summary>
    [Fact]
    public async Task AllRooms_ShouldBeInitializedAsync()
    {
        // Préparation : créer un contexte EF en mémoire et initialiser la base
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var rooms = await service.GetAllRoomsAsync();

        Assert.NotEmpty(rooms);
        foreach (var room in rooms)
        {
            Assert.NotNull(room);
            Assert.True(room.RoomId > 0, "Chaque salle doit avoir un RoomId valide.");
            Assert.False(string.IsNullOrWhiteSpace(room.Description), "Chaque salle doit avoir une description.");
            Assert.NotNull(room.Actions);
            Assert.NotEmpty(room.Actions);

            Assert.True(room.Monster != null || room.Chest != null,
                "Chaque salle doit contenir soit un monstre, soit un coffre.");

            if (room.Monster != null)
            {
                Assert.True(room.Monster.CharacterId > 0, "Le monstre doit avoir un CharacterId valide.");
                Assert.NotNull(room.Monster.Weapon);
            }

            if (room.Chest != null)
            {
                Assert.True(room.Chest.ChestId > 0, "Le coffre doit avoir un ChestId valide.");
                Assert.NotNull(room.Chest.Potion);
            }
        }
    }

    /// <summary>
    /// Vérifie que la première salle retournée par <see cref="RoomsService.GetAllRoomsAsync"/> 
    /// possède les bonnes propriétés, y compris l'identifiant, la description, le monstre ou le coffre, et les actions disponibles.
    /// </summary>
    [Fact]
    public async Task NavigateToFirstRoom_ShouldNotBeEmpty()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var rooms = await service.GetAllRoomsAsync();
        Assert.NotEmpty(rooms);

        var firstRoom = rooms[0];

        Assert.True(firstRoom.RoomId > 0, "La première salle doit avoir un RoomId valide.");
        Assert.False(string.IsNullOrWhiteSpace(firstRoom.Description), "La première salle doit avoir une description.");
        Assert.NotNull(firstRoom.Actions);
        Assert.NotEmpty(firstRoom.Actions);

        Assert.True(firstRoom.Monster != null || firstRoom.Chest != null,
            "La première salle doit contenir soit un monstre, soit un coffre.");

        if (firstRoom.Monster != null)
        {
            Assert.True(firstRoom.Monster.CharacterId > 0, "Le monstre doit avoir un CharacterId valide.");
            Assert.NotNull(firstRoom.Monster.Weapon);
        }

        if (firstRoom.Chest != null)
        {
            Assert.True(firstRoom.Chest.ChestId > 0, "Le coffre doit avoir un ChestId valide.");
            Assert.NotNull(firstRoom.Chest.Potion);
        }
    }

    /// <summary>
    /// Vérifie que GetAllRoomsAsync retourne toutes les salles.
    /// </summary>
    [Fact]
    public async Task GetAllRoomsAsync_ShouldReturnAllRooms()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var rooms = await service.GetAllRoomsAsync();

        Assert.NotNull(rooms);
        Assert.NotEmpty(rooms);
        Assert.Equal(2, rooms.Count);
    }

    /// <summary>
    /// Vérifie que GetRoomByIdAsync retourne la salle correcte si elle existe.
    /// </summary>
    [Fact]
    public async Task GetRoomByIdAsync_ShouldReturnRoom_WhenExists()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var room = await service.GetRoomByIdAsync(1);

        Assert.NotNull(room);
        Assert.Equal(1, room.RoomId);
    }

    /// <summary>
    /// Vérifie que GetRoomByIdAsync retourne null si la salle n'existe pas.
    /// </summary>
    [Fact]
    public async Task GetRoomByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var room = await service.GetRoomByIdAsync(999);

        Assert.Null(room);
    }

    /// <summary>
    /// Vérifie que les salles ont des IDs uniques.
    /// </summary>
    [Fact]
    public async Task GetAllRoomsAsync_ShouldReturnRoomsWithUniqueIds()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var rooms = await service.GetAllRoomsAsync();
        var roomIds = rooms.Select(r => r.RoomId).ToList();

        Assert.Equal(roomIds.Distinct().Count(), roomIds.Count);
    }

    /// <summary>
    /// Vérifie que chaque salle a une liste d'actions non vide.
    /// </summary>
    [Fact]
    public async Task GetAllRoomsAsync_EachRoom_ShouldHaveActions()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var rooms = await service.GetAllRoomsAsync();

        Assert.All(rooms, room =>
        {
            Assert.NotNull(room.Actions);
            Assert.NotEmpty(room.Actions);
        });
    }

    /// <summary>
    /// Vérifie que GetRoomByIdAsync retourne la salle avec son ID correct.
    /// </summary>
    [Fact]
    public async Task GetRoomByIdAsync_ShouldReturnCorrectRoom()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);
        var expectedRoom = context.Rooms.First();

        var room = await service.GetRoomByIdAsync(expectedRoom.RoomId);

        Assert.NotNull(room);
        Assert.Equal(expectedRoom.Description, room.Description);
    }

    /// <summary>
    /// Vérifie que les salles peuvent avoir des monstres.
    /// </summary>
    [Fact]
    public async Task GetAllRoomsAsync_ShouldContainMonsterRooms()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var rooms = await service.GetAllRoomsAsync();
        var monstersRooms = rooms.Where(r => r.Monster != null);

        Assert.NotEmpty(monstersRooms);
    }

    /// <summary>
    /// Vérifie que les salles peuvent avoir des coffres.
    /// </summary>
    [Fact]
    public async Task GetAllRoomsAsync_ShouldContainChestRooms()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var rooms = await service.GetAllRoomsAsync();
        var chestRooms = rooms.Where(r => r.Chest != null);

        Assert.NotEmpty(chestRooms);
    }

    /// <summary>
    /// Vérifie que GetRoomByIdAsync avec un ID valide ne retourne pas null.
    /// </summary>
    [Fact]
    public async Task GetRoomByIdAsync_ValidId_ShouldNotReturnNull()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);
        var firstRoom = context.Rooms.First();

        var room = await service.GetRoomByIdAsync(firstRoom.RoomId);

        Assert.NotNull(room);
    }

    /// <summary>
    /// Vérifie que les descriptions de salles sont non nulles.
    /// </summary>
    [Fact]
    public async Task GetAllRoomsAsync_Descriptions_ShouldNotBeNull()
    {
        using var context = CreateInMemoryContext();
        var service = new RoomsService(context);

        var rooms = await service.GetAllRoomsAsync();

        Assert.All(rooms, room =>
        {
            Assert.NotNull(room.Description);
            Assert.NotEmpty(room.Description);
        });
    }
}
