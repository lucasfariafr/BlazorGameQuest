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
    
    [Fact]
    public void GetAllRooms_ShouldReturnOkWithRooms()
    {
        // Arrange
        var mockService = new Mock<IRoomService>();
        mockService.Setup(s => s.GetAllRooms()).Returns(new List<Room>
        {
            new() { Id = 1, Description = "Room 1"  , AvailableActions = new List<PlayerAction>()}
        });

        var controller = new RoomController(mockService.Object);
        
        var result = controller.GetAllRooms();
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var rooms = Assert.IsAssignableFrom<IEnumerable<Room>>(okResult.Value);
        Assert.Single(rooms);
    }
    
    [Fact]
    public void FirstRoom_ShouldBeFirstRoom()
    {
        var roomInitializer = new RoomInitializer();
        var service = new RoomService(roomInitializer);
        
        var rooms = service.GetAllRooms();
        var firstRoom = rooms.First();
        
        Assert.Equal(1, firstRoom.Id);
        Assert.Equal($"Un {MonsterType.Goblin} apparaît. Que faites-vous ?", firstRoom.Description);
        Assert.Equal(MonsterType.Goblin, firstRoom.Monster);

        var expectedActions = new List<PlayerAction>
        {
            PlayerAction.Fight,
            PlayerAction.RunAway,
            PlayerAction.Search
        };
        Assert.Equal(expectedActions, firstRoom.AvailableActions);
    }
    
    [Fact]
    public void GetRoomById_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        var mockService = new Mock<IRoomService>();
        mockService.Setup(s => s.GetRoomById(It.IsAny<int>())).Returns((Room)null);

        var controller = new RoomController(mockService.Object);
        
        var result = controller.GetRoomById(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("n'existe pas", notFoundResult.Value.ToString());
    }

    
}