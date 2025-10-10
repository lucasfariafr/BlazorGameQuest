using GameServices.Interfaces;
using GameServices.Services;

namespace GameTest;

using GameServices.Interfaces;
using GameServices.Services;
using SharedModels.Models;
using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
{
    public class RoomServiceTests
    {
        [Fact]
        public void GetAllRooms_ShouldReturnAllRooms()
        {
            var rooms = new List<Room>
            {
                new() { Id = 1, Description = "Room 1" },
                new() { Id = 2, Description = "Room 2" }
            };

            var mockInitializer = new Mock<IRoomInitializer>();
            mockInitializer.Setup(x => x.InitializeRooms()).Returns(rooms);

            var service = new RoomService(mockInitializer.Object);

            // Act
            var result = service.GetAllRooms();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Room 1", result.First().Description);
        }

        [Fact]
        public void GetRoomById_ShouldReturnRoom_WhenExists()
        {
            var rooms = new List<Room>
            {
                new() { Id = 1, Description = "Room 1" },
                new() { Id = 2, Description = "Room 2" }
            };

            var mockInitializer = new Mock<IRoomInitializer>();
            mockInitializer.Setup(x => x.InitializeRooms()).Returns(rooms);

            var service = new RoomService(mockInitializer.Object);
            
            var room = service.GetRoomById(2);
            
            Assert.NotNull(room);
            Assert.Equal(2, room.Id);
            Assert.Equal("Room 2", room.Description);
        }

        [Fact]
        public void GetRoomById_ShouldReturnNull_WhenNotFound()
        {
            var rooms = new List<Room>
            {
                new() { Id = 1, Description = "Room 1" }
            };

            var mockInitializer = new Mock<IRoomInitializer>();
            mockInitializer.Setup(x => x.InitializeRooms()).Returns(rooms);

            var service = new RoomService(mockInitializer.Object);
            
            var result = service.GetRoomById(999);
            
            Assert.Null(result);
        }
    }
}
