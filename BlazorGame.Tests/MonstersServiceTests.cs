using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using BlazorGame.SharedModels.Enums.Entities;
using Microsoft.EntityFrameworkCore;


namespace BlazorGame.Tests
{
    public class MonstersServiceTests
    {
        private GameDatabaseContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GameDatabaseContext>()
                .UseInMemoryDatabase("GameDb_MonsterTest_" + Guid.NewGuid())
                .Options;

            var context = new GameDatabaseContext(options);
            DatabaseInitializer.Initialize(context);
            return context;
        }

        [Fact]
        public async Task GetAllMonstersAsync_ShouldReturnAll()
        {
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);

            var monsters = await service.GetAllMonstersAsync();

            Assert.NotEmpty(monsters);
            Assert.All(monsters, m => Assert.NotNull(m.Weapon));
        }

        [Fact]
        public async Task GetMonsterByIdAsync_ShouldReturnMonster_WhenExists()
        {
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);
            var existing = context.Monsters.First();

            var monster = await service.GetMonsterByIdAsync(existing.CharacterId);

            Assert.NotNull(monster);
            Assert.Equal(existing.CharacterId, monster.CharacterId);
        }

        [Fact]
        public async Task GetMonstersByTypeAsync_ShouldReturnOnlyThatType()
        {
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);

            var zombies = await service.GetMonstersByTypeAsync(MonsterType.Zombie);

            Assert.All(zombies, z => Assert.Equal(MonsterType.Zombie, z.Type));
        }

        [Fact]
        public async Task UpdateMonsterHealthAsync_ShouldUpdateHealth()
        {
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);
            var monster = context.Monsters.First();

            var ok = await service.UpdateMonsterHealthAsync(monster.CharacterId, 5);

            Assert.True(ok);
            Assert.Equal(5, (await context.Monsters.FindAsync(monster.CharacterId)).Health);
        }

        [Fact]
        public async Task MonsterExistsAsync_ShouldReturnTrue_WhenExists()
        {
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);
            var id = context.Monsters.First().CharacterId;

            Assert.True(await service.MonsterExistsAsync(id));
        }

        [Fact]
        public async Task IsMonsterAliveAsync_ShouldReturnFalse_WhenHealthZero()
        {
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);
            var monster = context.Monsters.First();
            monster.Health = 0;
            context.SaveChanges();

            Assert.False(await service.IsMonsterAliveAsync(monster.CharacterId));
        }
    }
}
