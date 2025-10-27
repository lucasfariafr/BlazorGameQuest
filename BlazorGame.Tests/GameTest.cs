using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using BlazorGame.SharedModels.Enums.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Tests;

public class GameTest
{
    public class MonstersServiceTests
    {
        // 🔧 Utilitaire pour créer un contexte EF Core InMemory et initialiser la BD
        private GameDatabaseContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GameDatabaseContext>()
                .UseInMemoryDatabase(databaseName: "GameDb_MonstersTest_" + Guid.NewGuid())
                .Options;

            var context = new GameDatabaseContext(options);
            DatabaseInitializer.Initialize(context);
            return context;
        }

        [Fact]
        public async Task GetAllMonstersAsync_ShouldReturnAllMonsters()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);

            // Act
            var monsters = await service.GetAllMonstersAsync();

            // Assert
            Assert.NotNull(monsters);
            Assert.NotEmpty(monsters);
            Assert.True(monsters.Count >= 2); // zombie + goblin
            Assert.All(monsters, m => Assert.NotNull(m.Weapon)); // chaque monstre a une arme
        }

        [Fact]
        public async Task GetMonsterByIdAsync_ShouldReturnMonster_WhenExists()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);
            var knownMonster = context.Monsters.First();

            // Act
            var monster = await service.GetMonsterByIdAsync(knownMonster.CharacterId);

            // Assert
            Assert.NotNull(monster);
            Assert.Equal(knownMonster.CharacterId, monster.CharacterId);
            Assert.NotNull(monster.Weapon);
        }

        [Fact]
        public async Task GetMonsterByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);

            // Act
            var result = await service.GetMonsterByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetMonstersByTypeAsync_ShouldReturnOnlyMatchingType()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);

            // Act
            var zombies = await service.GetMonstersByTypeAsync(MonsterType.Zombie);

            // Assert
            Assert.NotEmpty(zombies);
            Assert.All(zombies, m => Assert.Equal(MonsterType.Zombie, m.Type));
        }

        [Fact]
        public async Task UpdateMonsterHealthAsync_ShouldUpdateHealth_WhenMonsterExists()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);
            var monster = context.Monsters.First();

            // Act
            var success = await service.UpdateMonsterHealthAsync(monster.CharacterId, 15);

            // Assert
            Assert.True(success);

            var updated = await context.Monsters.FindAsync(monster.CharacterId);
            Assert.Equal(15, updated.Health);
        }

        [Fact]
        public async Task UpdateMonsterHealthAsync_ShouldReturnFalse_WhenNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);

            // Act
            var result = await service.UpdateMonsterHealthAsync(999, 20);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task MonsterExistsAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);
            var monsterId = context.Monsters.First().CharacterId;

            // Act
            var exists = await service.MonsterExistsAsync(monsterId);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task MonsterExistsAsync_ShouldReturnFalse_WhenNotExists()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);

            // Act
            var exists = await service.MonsterExistsAsync(999);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task IsMonsterAliveAsync_ShouldReturnTrue_WhenHealthAboveZero()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);
            var monster = context.Monsters.First();
            monster.Health = 10;
            context.SaveChanges();

            // Act
            var isAlive = await service.IsMonsterAliveAsync(monster.CharacterId);

            // Assert
            Assert.True(isAlive);
        }

        [Fact]
        public async Task IsMonsterAliveAsync_ShouldReturnFalse_WhenHealthZeroOrBelow()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new MonstersService(context);
            var monster = context.Monsters.First();
            monster.Health = 0;
            context.SaveChanges();

            // Act
            var isAlive = await service.IsMonsterAliveAsync(monster.CharacterId);

            // Assert
            Assert.False(isAlive);
        }
    }
}