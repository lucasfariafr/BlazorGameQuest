using System;
using System.Linq;
using System.Threading.Tasks;
using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BlazorGame.Tests
{
    public class RoomsServiceTests
    {
        private GameDatabaseContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GameDatabaseContext>()
                .UseInMemoryDatabase("GameDb_RoomTest_" + Guid.NewGuid())
                .Options;

            var context = new GameDatabaseContext(options);
            DatabaseInitializer.Initialize(context);
            return context;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRooms()
        {
            using var context = CreateInMemoryContext();
            var service = new RoomsService(context);

            var rooms = await service.GetAllAsync();

            Assert.NotNull(rooms);
            Assert.NotEmpty(rooms);
            Assert.Equal(2, rooms.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRoom_WhenExists()
        {
            using var context = CreateInMemoryContext();
            var service = new RoomsService(context);

            var room = await service.GetByIdAsync(1);

            Assert.NotNull(room);
            Assert.Equal(1, room.RoomId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            using var context = CreateInMemoryContext();
            var service = new RoomsService(context);

            var room = await service.GetByIdAsync(999);

            Assert.Null(room);
        }
    }
}