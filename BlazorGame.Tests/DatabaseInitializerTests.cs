using System.Linq;
using BlazorGame.GameService.Data;
using BlazorGame.SharedModels.Enums.Environment;
using BlazorGame.SharedModels.Models; 
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BlazorGame.Tests;

public class DatabaseInitializerTests
{
    private GameDatabaseContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GameDatabaseContext>()
                .UseInMemoryDatabase(databaseName: "GameDb_Test_" + System.Guid.NewGuid())
                .Options;

            return new GameDatabaseContext(options);
        }

        [Fact]
        public void Initialize_ShouldPopulateDatabase_WhenEmpty()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            DatabaseInitializer.Initialize(context);

            // Assert
            Assert.True(context.Weapons.Any(), "Weapons should be initialized.");
            Assert.True(context.Potions.Any(), "Potions should be initialized.");
            Assert.True(context.Player.Any(), "Players should be initialized.");
            Assert.True(context.Monsters.Any(), "Monsters should be initialized.");
            Assert.True(context.Chests.Any(), "Chests should be initialized.");
            Assert.True(context.Rooms.Any(), "Rooms should be initialized.");
            Assert.True(context.Dungeons.Any(), "Dungeons should be initialized.");

            // Example of checking a specific data property
            var dungeon = context.Dungeons
                .Include(d => d.Rooms)
                .FirstOrDefault();

            Assert.NotNull(dungeon);
            Assert.Equal(DungeonLevel.Easy, dungeon.DifficultyLevel);
            Assert.False(dungeon.IsCompleted);
            Assert.Equal(2, dungeon.Rooms.Count);
        }

        [Fact]
        public void Initialize_ShouldNotDuplicateData_WhenAlreadyInitialized()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            DatabaseInitializer.Initialize(context);

            // Act
            DatabaseInitializer.Initialize(context);

            // Assert
            // The second initialization should not add new entries
            Assert.Single(context.Dungeons);
            Assert.Equal(3, context.Weapons.Count());
        }
}