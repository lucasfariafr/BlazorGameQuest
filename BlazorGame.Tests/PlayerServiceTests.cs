using System;
using System.Linq;
using System.Threading.Tasks;
using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GameTest
{
    public class PlayerServiceTests
    {
        private GameDatabaseContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GameDatabaseContext>()
                .UseInMemoryDatabase("GameDb_PlayerTest_" + Guid.NewGuid())
                .Options;

            var context = new GameDatabaseContext(options);
            DatabaseInitializer.Initialize(context);
            return context;
        }

        [Fact]
        public async Task GetPlayerByIdAsync_ShouldReturnPlayer_WhenExists()
        {
            using var context = CreateInMemoryContext();
            var service = new PlayerService(context);
            var player = context.Player.First();

            var found = await service.GetPlayerByIdAsync(player.CharacterId);

            Assert.NotNull(found);
            Assert.Equal(player.CharacterId, found.CharacterId);
        }
        

        [Fact]
        public async Task UpdateHealthAsync_ShouldUpdateValue()
        {
            using var context = CreateInMemoryContext();
            var service = new PlayerService(context);
            var player = context.Player.First();

            await service.UpdatePlayerHealthAsync(player.CharacterId, 50);

            var updated = await context.Player.FindAsync(player.CharacterId);
            Assert.Equal(50, updated.Health);
        }

        [Fact]
        public async Task PlayerExistsAsync_ShouldReturnFalse_WhenNotFound()
        {
            using var context = CreateInMemoryContext();
            var service = new PlayerService(context);

            Assert.False(await service.PlayerExistsAsync(999));
        }
    }
}
