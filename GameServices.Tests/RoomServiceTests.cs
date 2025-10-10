using GameServices.Controllers;
using GameServices.Interfaces;
using GameServices.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedModels.Enums;
using SharedModels.Models;

namespace GameServices.Tests;

public class RoomServiceTests
{
    // Vérifie que GetAllRooms retourne un résultat Ok avec une liste de salles
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
                AvailableActions = []
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

    // Vérifie que la première salle retournée par le service possède les bonnes propriétés
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

    // Vérifie que GetRoomById retourne un résultat NotFound lorsque l'ID de la salle n'existe pas
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
