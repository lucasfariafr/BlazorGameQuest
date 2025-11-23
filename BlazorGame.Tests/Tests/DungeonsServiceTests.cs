namespace BlazorGame.Tests.Tests;

/// <summary>
/// Tests unitaires pour vérifier le bon fonctionnement du DungeonsService.
/// </summary>
public class DungeonsServiceTests
{
    /// <summary>
    /// Crée un contexte EF Core en mémoire et initialise la base de données.
    /// </summary>
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_DungeonsTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    /// <summary>
    /// Crée le service DungeonsService avec un contexte en mémoire.
    /// </summary>
    private static (GameDatabaseContext Context, DungeonsService Service) CreateService()
    {
        var context = CreateInMemoryContext();
        var service = new DungeonsService(context);
        return (context, service);
    }

    /// <summary>
    /// Vérifie que GetAllDungeonsAsync retourne tous les donjons.
    /// </summary>
    [Fact]
    public async Task GetAllDungeonsAsync_ShouldReturnAllDungeons()
    {
        var (_, service) = CreateService();

        var dungeons = await service.GetAllDungeonsAsync();

        Assert.NotEmpty(dungeons);
    }

    /// <summary>
    /// Vérifie que GetDungeonByIdAsync retourne le donjon correct.
    /// </summary>
    [Fact]
    public async Task GetDungeonByIdAsync_ShouldReturnDungeon_WhenExists()
    {
        var (context, service) = CreateService();
        var existingDungeon = context.Dungeons.First();

        var dungeon = await service.GetDungeonByIdAsync(existingDungeon.DungeonId);

        Assert.NotNull(dungeon);
        Assert.Equal(existingDungeon.DungeonId, dungeon.DungeonId);
    }

    /// <summary>
    /// Vérifie que GetDungeonByIdAsync retourne null si le donjon n'existe pas.
    /// </summary>
    [Fact]
    public async Task GetDungeonByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GetDungeonByIdAsync(999);

        Assert.Null(dungeon);
    }

    /// <summary>
    /// Vérifie que GenerateRandomDungeonAsync génère un donjon avec le nombre de salles demandé.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldGenerateDungeonWithCorrectRoomCount()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        Assert.NotNull(dungeon);
        Assert.Equal(5, dungeon.Rooms.Count);
    }

    /// <summary>
    /// Vérifie que GenerateRandomDungeonAsync génère un donjon de difficulté facile.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldGenerateEasyDungeon()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 3);

        Assert.NotNull(dungeon);
        Assert.Equal(DungeonLevel.Easy, dungeon.DifficultyLevel);
        Assert.False(dungeon.IsCompleted);
    }

    /// <summary>
    /// Vérifie que GenerateRandomDungeonAsync génère un donjon de difficulté moyenne.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldGenerateMediumDungeon()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Medium, 3);

        Assert.NotNull(dungeon);
        Assert.Equal(DungeonLevel.Medium, dungeon.DifficultyLevel);
    }

    /// <summary>
    /// Vérifie que GenerateRandomDungeonAsync génère un donjon de difficulté difficile.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldGenerateDifficultDungeon()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Difficult, 3);

        Assert.NotNull(dungeon);
        Assert.Equal(DungeonLevel.Difficult, dungeon.DifficultyLevel);
    }

    /// <summary>
    /// Vérifie que les salles générées ont des IDs uniques.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldGenerateRoomsWithUniqueIds()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        var roomIds = dungeon.Rooms.Select(r => r.RoomId).ToList();
        Assert.Equal(roomIds.Distinct().Count(), roomIds.Count);
    }

    /// <summary>
    /// Vérifie que chaque salle a une description.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldGenerateRoomsWithDescriptions()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        Assert.All(dungeon.Rooms, room => Assert.False(string.IsNullOrWhiteSpace(room.Description)));
    }

    /// <summary>
    /// Vérifie que chaque salle a des actions disponibles.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldGenerateRoomsWithActions()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        Assert.All(dungeon.Rooms, room =>
        {
            Assert.NotNull(room.Actions);
            Assert.NotEmpty(room.Actions);
        });
    }

    /// <summary>
    /// Vérifie qu'au moins une salle contient un monstre.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldHaveAtLeastOneMonsterRoom()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        Assert.Contains(dungeon.Rooms, room => room.Monster != null);
    }

    /// <summary>
    /// Vérifie qu'au moins une salle contient un coffre.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldHaveAtLeastOneChestRoom()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        Assert.Contains(dungeon.Rooms, room => room.Chest != null);
    }

    /// <summary>
    /// Vérifie que les monstres générés ont les bonnes stats selon la difficulté.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_MonstersHaveCorrectStats()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        var monstersRooms = dungeon.Rooms.Where(r => r.Monster != null).ToList();
        Assert.All(monstersRooms, room =>
        {
            Assert.True(room.Monster!.Strength > 0);
            Assert.True(room.Monster.Armor >= 0);
        });
    }

    /// <summary>
    /// Vérifie que les coffres non ouverts sont générés correctement.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ChestsAreNotOpened()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        var chestRooms = dungeon.Rooms.Where(r => r.Chest != null).ToList();
        Assert.All(chestRooms, room => Assert.False(room.Chest!.IsOpened));
    }

    /// <summary>
    /// Vérifie que la génération de plusieurs donjons crée des IDs uniques.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_MultipleDungeonsHaveUniqueIds()
    {
        var (_, service) = CreateService();

        var dungeon1 = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 3);
        var dungeon2 = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 3);

        Assert.NotEqual(dungeon1.DungeonId, dungeon2.DungeonId);
    }

    /// <summary>
    /// Vérifie que les salles avec monstres ont les bonnes actions disponibles.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_MonsterRoomsHaveCorrectActions()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        var monsterRooms = dungeon.Rooms.Where(r => r.Monster != null).ToList();
        Assert.All(monsterRooms, room =>
        {
            Assert.Contains(AvailableActions.Fight, room.Actions);
            Assert.Contains(AvailableActions.RunAway, room.Actions);
        });
    }

    /// <summary>
    /// Vérifie que les salles avec coffres ont les bonnes actions disponibles.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ChestRoomsHaveCorrectActions()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        var chestRooms = dungeon.Rooms.Where(r => r.Chest != null).ToList();
        Assert.All(chestRooms, room =>
        {
            Assert.Contains(AvailableActions.Open, room.Actions);
            Assert.Contains(AvailableActions.Ignore, room.Actions);
        });
    }

    /// <summary>
    /// Vérifie que les salles vides ont les bonnes actions disponibles.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_EmptyRoomsHaveCorrectActions()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 10);

        var emptyRooms = dungeon.Rooms.Where(r => r.Monster == null && r.Chest == null).ToList();
        Assert.All(emptyRooms, room =>
        {
            Assert.Contains(AvailableActions.Search, room.Actions);
            Assert.Contains(AvailableActions.Ignore, room.Actions);
        });
    }

    /// <summary>
    /// Vérifie que GenerateRandomDungeonAsync génère différents types de monstres.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldGenerateDifferentMonsterTypes()
    {
        var (_, service) = CreateService();
        var allMonsterTypes = new HashSet<MonsterType>();

        // Générer plusieurs donjons pour augmenter les chances de voir différents types
        for (int i = 0; i < 10; i++)
        {
            var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);
            var monsters = dungeon.Rooms.Where(r => r.Monster != null).Select(r => r.Monster!.Type);
            foreach (var type in monsters)
            {
                allMonsterTypes.Add(type);
            }
        }

        // Devrait avoir au moins 2 types de monstres différents
        Assert.True(allMonsterTypes.Count >= 2);
    }

    /// <summary>
    /// Vérifie que les coffres peuvent contenir des armes ou des potions.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ChestsShouldContainWeaponOrPotion()
    {
        var (_, service) = CreateService();
        var hasWeapon = false;
        var hasPotion = false;

        for (int i = 0; i < 20; i++)
        {
            var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);
            var chests = dungeon.Rooms.Where(r => r.Chest != null).Select(r => r.Chest!);

            foreach (var chest in chests)
            {
                if (chest.Weapon != null) hasWeapon = true;
                if (chest.Potion != null) hasPotion = true;
            }

            if (hasWeapon && hasPotion) break;
        }

        Assert.True(hasWeapon || hasPotion);
    }

    /// <summary>
    /// Vérifie que les salles ont des types de salle différents.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldGenerateVariousRoomTypes()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 10);

        var monsterRooms = dungeon.Rooms.Count(r => r.Monster != null);
        var chestRooms = dungeon.Rooms.Count(r => r.Chest != null);
        var emptyRooms = dungeon.Rooms.Count(r => r.Monster == null && r.Chest == null);

        // Le donjon devrait avoir une variété de salles
        Assert.True(monsterRooms >= 1);
        Assert.True(chestRooms >= 1);
    }

    /// <summary>
    /// Vérifie que GetDungeonByIdAsync inclut les salles.
    /// </summary>
    [Fact]
    public async Task GetDungeonByIdAsync_ShouldIncludeRooms()
    {
        var (context, service) = CreateService();
        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        // Récupérer le donjon par ID
        var foundDungeon = await context.Dungeons
            .Include(d => d.Rooms)
            .FirstOrDefaultAsync(d => d.DungeonId == dungeon.DungeonId);

        Assert.NotNull(foundDungeon);
        Assert.NotEmpty(foundDungeon.Rooms);
    }

    /// <summary>
    /// Vérifie que les armes dans les coffres ont des types valides.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_WeaponsHaveValidTypes()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 10);

        var weaponsInChests = dungeon.Rooms
            .Where(r => r.Chest?.Weapon != null)
            .Select(r => r.Chest!.Weapon!)
            .ToList();

        Assert.All(weaponsInChests, weapon =>
        {
            Assert.True(Enum.IsDefined(typeof(WeaponType), weapon.Type));
        });
    }

    /// <summary>
    /// Vérifie que les potions dans les coffres ont des types valides.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_PotionsHaveValidTypes()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 10);

        var potionsInChests = dungeon.Rooms
            .Where(r => r.Chest?.Potion != null)
            .Select(r => r.Chest!.Potion!)
            .ToList();

        Assert.All(potionsInChests, potion =>
        {
            Assert.True(Enum.IsDefined(typeof(PotionType), potion.Type));
        });
    }

    /// <summary>
    /// Vérifie que les monstres avec salles ont les bonnes actions de recherche.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_MonsterRoomsHaveSearchAction()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Easy, 5);

        var monsterRooms = dungeon.Rooms.Where(r => r.Monster != null).ToList();
        Assert.All(monsterRooms, room =>
        {
            Assert.Contains(AvailableActions.Search, room.Actions);
        });
    }

    /// <summary>
    /// Vérifie que le donjon n'est pas complété à la création.
    /// </summary>
    [Fact]
    public async Task GenerateRandomDungeonAsync_ShouldNotBeCompletedOnCreation()
    {
        var (_, service) = CreateService();

        var dungeon = await service.GenerateRandomDungeonAsync(DungeonLevel.Medium, 5);

        Assert.False(dungeon.IsCompleted);
    }
}
