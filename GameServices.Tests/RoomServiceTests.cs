using GameServices.Controllers;
using GameServices.Interfaces;
using GameServices.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedModels.Enums;
using SharedModels.Models;

namespace GameServices.Tests;

/// <summary>
/// Contient les tests unitaires pour le service <see cref="RoomService"/> et le contrôleur <see cref="RoomController"/>.
/// </summary>
public class RoomServiceTests
{
    /// <summary>
    /// Vérifie que la méthode <see cref="RoomController.GetAllRooms"/> retourne un résultat <see cref="OkObjectResult"/>
    /// contenant une liste de salles non vide.
    /// </summary>
    [Fact]
    public void GetAllRooms_ShouldReturnOkWithRooms()
    {
        // Préparation
        var mockService = new Mock<IRoomService>();
        var expectedRooms = new List<Room>
            {
                new()
                {
                    Id = 1,
                    Description = "Room 1",
                    AvailableActions = new List<PlayerAction>()
                }
            };
        mockService.Setup(s => s.GetAllRooms()).Returns(expectedRooms);
        var controller = new RoomController(mockService.Object);

        // Action
        var result = controller.GetAllRooms();

        // Vérification
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var rooms = Assert.IsAssignableFrom<IEnumerable<Room>>(okResult.Value);
        Assert.Single(rooms);
    }

    /// <summary>
    /// Vérifie que la première salle retournée par <see cref="RoomService.GetAllRooms"/>
    /// possède les bonnes propriétés, y compris l'identifiant, la description, le type de monstre
    /// et les actions disponibles.
    /// </summary>
    [Fact]
    public void NavigateToFirstRoom_ShouldBeFirstRoom()
    {
        // Préparation
        var roomInitializer = new RoomInitializer();
        var service = new RoomService(roomInitializer);
        var expectedActions = new List<PlayerAction>
            {
                PlayerAction.Fight,
                PlayerAction.RunAway,
                PlayerAction.Search
            };

        // Action
        var rooms = service.GetAllRooms();
        var firstRoom = rooms[0];

        // Vérification
        Assert.Equal(1, firstRoom.Id);
        Assert.Equal($"Un {MonsterType.Goblin} apparaît. Que faites-vous ?", firstRoom.Description);
        Assert.Equal(MonsterType.Goblin, firstRoom.Monster);
        Assert.Equal(expectedActions, firstRoom.AvailableActions);
    }

    /// <summary>
    /// Vérifie que <see cref="RoomController.GetRoomById(int)"/> retourne un résultat <see cref="NotFoundObjectResult"/>
    /// lorsque l'identifiant de la salle n'existe pas.
    /// </summary>
    [Fact]
    public void GetRoomById_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        // Préparation
        const int nonExistentRoomId = 999;
        var mockService = new Mock<IRoomService>();
        mockService.Setup(s => s.GetRoomById(It.IsAny<int>())).Returns((Room?)null);
        var controller = new RoomController(mockService.Object);

        // Action
        var result = controller.GetRoomById(nonExistentRoomId);

        // Vérification
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.NotNull(notFoundResult.Value);
        Assert.Contains($"La salle avec l'ID {nonExistentRoomId} n'existe pas.", notFoundResult.Value.ToString());
    }
}
