using System;
using System.Linq;
using System.Threading.Tasks;
using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using Microsoft.EntityFrameworkCore;
using BlazorGame.SharedModels.Models;
using BlazorGame.SharedModels.Models.Entities;
using Xunit;

namespace GameTest
{
    public class FightServiceTests
    {
        private GameDatabaseContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GameDatabaseContext>()
                .UseInMemoryDatabase("GameDb_FightTest_" + Guid.NewGuid())
                .Options;

            var context = new GameDatabaseContext(options);
            DatabaseInitializer.Initialize(context);
            return context;
        }

        private (FightService service, Player player, Monster monster) Setup(GameDatabaseContext context)
        {
            var playerService = new PlayerService(context);
            var monsterService = new MonstersService(context);
            var fightService = new FightService(playerService, monsterService, context);

            var player = context.Player.First();
            var monster = context.Monsters.First();
            player.Health = 100;
            monster.Health = 50;
            context.SaveChanges();

            return (fightService, player, monster);
        }

        [Fact]
        public async Task ExecuteFightAsync_ShouldReturnVictory()
        {
            using var context = CreateInMemoryContext();
            var (service, player, monster) = Setup(context);

            player.Strength = 40;
            monster.Strength = 5;
            context.SaveChanges();

            var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

            Assert.NotNull(result);
            Assert.Contains("Victoire", result.Result);
        }

        [Fact]
        public async Task ExecuteFightAsync_ShouldReturnDefeat()
        {
            using var context = CreateInMemoryContext();
            var (service, player, monster) = Setup(context);

            player.Strength = 5;
            monster.Strength = 40;
            context.SaveChanges();

            var result = await service.ExecuteFightAsync(player.CharacterId, monster.CharacterId);

            Assert.Contains("Défaite", result.Result);
        }

        [Fact]
        public async Task ExecuteFightAsync_ShouldThrow_WhenPlayerMissing()
        {
            using var context = CreateInMemoryContext();
            var playerService = new PlayerService(context);
            var monsterService = new MonstersService(context);
            var service = new FightService(playerService, monsterService, context);
            var monster = context.Monsters.First();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ExecuteFightAsync(999, monster.CharacterId));
        }

        [Fact]
        public async Task ExecuteFightAsync_ShouldThrow_WhenMonsterMissing()
        {
            using var context = CreateInMemoryContext();
            var playerService = new PlayerService(context);
            var monsterService = new MonstersService(context);
            var service = new FightService(playerService, monsterService, context);
            var player = context.Player.First();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ExecuteFightAsync(player.CharacterId, 999));
        }
    }
}
