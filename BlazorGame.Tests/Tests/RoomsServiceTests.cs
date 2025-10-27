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
}
