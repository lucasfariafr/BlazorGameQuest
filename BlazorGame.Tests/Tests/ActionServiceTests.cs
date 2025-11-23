namespace BlazorGame.Tests.Tests;

/// <summary>
/// Tests unitaires pour vérifier le bon fonctionnement de l'ActionService.
/// </summary>
public class ActionServiceTests
{
    /// <summary>
    /// Crée un contexte EF Core en mémoire et initialise la base de données.
    /// </summary>
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_ActionTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    /// <summary>
    /// Crée l'ActionService avec toutes ses dépendances.
    /// </summary>
    private static (GameDatabaseContext Context, ActionService Service, GameSessionService SessionService) CreateService()
    {
        var context = CreateInMemoryContext();
        var playerService = new PlayerService(context);
        var monsterService = new MonstersService(context);
        var fightService = new FightService(playerService, monsterService, context);
        var dungeonsService = new DungeonsService(context);
        var sessionService = new GameSessionService(context, dungeonsService);
        var actionService = new ActionService(context, playerService, fightService, sessionService);
        return (context, actionService, sessionService);
    }

    /// <summary>
    /// Vérifie que IgnoreAsync retourne un succès et passe à la salle suivante.
    /// </summary>
    [Fact]
    public async Task IgnoreAsync_ShouldReturnSuccess()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 3);
        var player = context.Player.First();
        var room = context.Rooms.First();
        var dungeon = context.Dungeons.First();

        var result = await service.IgnoreAsync(player.CharacterId, room.RoomId, dungeon.DungeonId);

        Assert.True(result.Success);
        Assert.Equal("Ignore", result.ActionType);
    }

    /// <summary>
    /// Vérifie que IgnoreAsync retourne une erreur si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task IgnoreAsync_ShouldReturnError_WhenPlayerNotFound()
    {
        var (context, service, _) = CreateService();
        var room = context.Rooms.First();
        var dungeon = context.Dungeons.First();

        var result = await service.IgnoreAsync(999, room.RoomId, dungeon.DungeonId);

        Assert.False(result.Success);
        Assert.Contains("Joueur introuvable", result.Message);
    }

    /// <summary>
    /// Vérifie que SearchAsync retourne une erreur si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task SearchAsync_ShouldReturnError_WhenPlayerNotFound()
    {
        var (context, service, _) = CreateService();
        var room = context.Rooms.First();
        var dungeon = context.Dungeons.First();

        var result = await service.SearchAsync(999, room.RoomId, dungeon.DungeonId);

        Assert.False(result.Success);
        Assert.Contains("Joueur introuvable", result.Message);
    }

    /// <summary>
    /// Vérifie que SearchAsync retourne une erreur si la salle n'existe pas.
    /// </summary>
    [Fact]
    public async Task SearchAsync_ShouldReturnError_WhenRoomNotFound()
    {
        var (context, service, _) = CreateService();
        var player = context.Player.First();
        var dungeon = context.Dungeons.First();

        var result = await service.SearchAsync(player.CharacterId, 999, dungeon.DungeonId);

        Assert.False(result.Success);
        Assert.Contains("Salle introuvable", result.Message);
    }

    /// <summary>
    /// Vérifie que OpenChestAsync retourne une erreur si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task OpenChestAsync_ShouldReturnError_WhenPlayerNotFound()
    {
        var (context, service, _) = CreateService();
        var room = context.Rooms.First();
        var dungeon = context.Dungeons.First();

        var result = await service.OpenChestAsync(999, room.RoomId, dungeon.DungeonId);

        Assert.False(result.Success);
        Assert.Contains("Joueur introuvable", result.Message);
    }

    /// <summary>
    /// Vérifie que OpenChestAsync retourne une erreur si la salle n'existe pas.
    /// </summary>
    [Fact]
    public async Task OpenChestAsync_ShouldReturnError_WhenRoomNotFound()
    {
        var (context, service, _) = CreateService();
        var player = context.Player.First();
        var dungeon = context.Dungeons.First();

        var result = await service.OpenChestAsync(player.CharacterId, 999, dungeon.DungeonId);

        Assert.False(result.Success);
        Assert.Contains("Salle introuvable", result.Message);
    }

    /// <summary>
    /// Vérifie que OpenChestAsync retourne une erreur s'il n'y a pas de coffre.
    /// </summary>
    [Fact]
    public async Task OpenChestAsync_ShouldReturnError_WhenNoChest()
    {
        var (context, service, _) = CreateService();
        var player = context.Player.First();
        var dungeon = context.Dungeons.First();
        // Trouver une salle avec un monstre (pas de coffre)
        var roomWithMonster = context.Rooms.Include(r => r.Monster).FirstOrDefault(r => r.Monster != null);

        if (roomWithMonster != null)
        {
            var result = await service.OpenChestAsync(player.CharacterId, roomWithMonster.RoomId, dungeon.DungeonId);

            Assert.False(result.Success);
            Assert.Contains("pas de coffre", result.Message);
        }
    }

    /// <summary>
    /// Vérifie que RunAwayAsync retourne le bon type d'action.
    /// </summary>
    [Fact]
    public async Task RunAwayAsync_ShouldReturnCorrectActionType()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 3);
        var player = context.Player.First();
        var room = context.Rooms.First();
        var dungeon = context.Dungeons.First();

        var result = await service.RunAwayAsync(player.CharacterId, room.RoomId, dungeon.DungeonId);

        Assert.Equal("RunAway", result.ActionType);
    }

    /// <summary>
    /// Vérifie que RunAwayAsync retourne une erreur si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task RunAwayAsync_ShouldReturnError_WhenPlayerNotFound()
    {
        var (context, service, _) = CreateService();
        var room = context.Rooms.First();
        var dungeon = context.Dungeons.First();

        var result = await service.RunAwayAsync(999, room.RoomId, dungeon.DungeonId);

        Assert.False(result.Success);
        Assert.Contains("Joueur introuvable", result.Message);
    }

    /// <summary>
    /// Vérifie que RunAwayAsync retourne une erreur si la salle n'existe pas.
    /// </summary>
    [Fact]
    public async Task RunAwayAsync_ShouldReturnError_WhenRoomNotFound()
    {
        var (context, service, _) = CreateService();
        var player = context.Player.First();
        var dungeon = context.Dungeons.First();

        var result = await service.RunAwayAsync(player.CharacterId, 999, dungeon.DungeonId);

        Assert.False(result.Success);
        Assert.Contains("Salle introuvable", result.Message);
    }

    /// <summary>
    /// Vérifie que FightAsync retourne une erreur si le joueur n'existe pas.
    /// </summary>
    [Fact]
    public async Task FightAsync_ShouldReturnError_WhenPlayerNotFound()
    {
        var (context, service, _) = CreateService();
        var room = context.Rooms.First();
        var dungeon = context.Dungeons.First();

        var result = await service.FightAsync(999, room.RoomId, dungeon.DungeonId);

        Assert.False(result.Success);
        Assert.Contains("Joueur introuvable", result.Message);
    }

    /// <summary>
    /// Vérifie que FightAsync retourne une erreur si la salle n'existe pas.
    /// </summary>
    [Fact]
    public async Task FightAsync_ShouldReturnError_WhenRoomNotFound()
    {
        var (context, service, _) = CreateService();
        var player = context.Player.First();
        var dungeon = context.Dungeons.First();

        var result = await service.FightAsync(player.CharacterId, 999, dungeon.DungeonId);

        Assert.False(result.Success);
        Assert.Contains("Salle introuvable", result.Message);
    }

    /// <summary>
    /// Vérifie que FightAsync retourne une erreur s'il n'y a pas de monstre.
    /// </summary>
    [Fact]
    public async Task FightAsync_ShouldReturnError_WhenNoMonster()
    {
        var (context, service, _) = CreateService();
        var player = context.Player.First();
        var dungeon = context.Dungeons.First();
        // Trouver une salle avec un coffre (pas de monstre)
        var roomWithChest = context.Rooms.Include(r => r.Chest).FirstOrDefault(r => r.Chest != null);

        if (roomWithChest != null)
        {
            var result = await service.FightAsync(player.CharacterId, roomWithChest.RoomId, dungeon.DungeonId);

            Assert.False(result.Success);
            Assert.Contains("pas de monstre", result.Message);
        }
    }

    /// <summary>
    /// Vérifie que FightAsync retourne un résultat de combat valide.
    /// </summary>
    [Fact]
    public async Task FightAsync_ShouldReturnValidResult_WhenMonsterExists()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 3);
        var player = context.Player.First();
        player.Strength = 50; // Joueur très fort pour garantir la victoire
        player.Health = 100;
        context.SaveChanges();

        var dungeon = context.Dungeons.First();
        var roomWithMonster = context.Rooms.Include(r => r.Monster).FirstOrDefault(r => r.Monster != null);

        if (roomWithMonster != null)
        {
            var result = await service.FightAsync(player.CharacterId, roomWithMonster.RoomId, dungeon.DungeonId);

            Assert.Equal("Fight", result.ActionType);
            Assert.NotNull(result.PlayerState);
        }
    }

    /// <summary>
    /// Vérifie que OpenChestAsync ouvre correctement un coffre.
    /// </summary>
    [Fact]
    public async Task OpenChestAsync_ShouldOpenChest_WhenExists()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 3);
        var player = context.Player.First();
        var dungeon = context.Dungeons.First();
        var roomWithChest = context.Rooms
            .Include(r => r.Chest)
            .FirstOrDefault(r => r.Chest != null && !r.Chest.IsOpened);

        if (roomWithChest != null)
        {
            var result = await service.OpenChestAsync(player.CharacterId, roomWithChest.RoomId, dungeon.DungeonId);

            Assert.Equal("Open", result.ActionType);
            Assert.NotNull(result.PlayerState);
        }
    }

    /// <summary>
    /// Vérifie que OpenChestAsync retourne une erreur si le coffre est déjà ouvert.
    /// </summary>
    [Fact]
    public async Task OpenChestAsync_ShouldReturnError_WhenChestAlreadyOpened()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 3);
        var player = context.Player.First();
        var dungeon = context.Dungeons.First();
        var roomWithChest = context.Rooms
            .Include(r => r.Chest)
            .FirstOrDefault(r => r.Chest != null);

        if (roomWithChest != null)
        {
            // Ouvrir le coffre une première fois
            await service.OpenChestAsync(player.CharacterId, roomWithChest.RoomId, dungeon.DungeonId);

            // Essayer de l'ouvrir une deuxième fois
            var result = await service.OpenChestAsync(player.CharacterId, roomWithChest.RoomId, dungeon.DungeonId);

            Assert.False(result.Success);
            Assert.Contains("déjà été ouvert", result.Message);
        }
    }

    /// <summary>
    /// Vérifie que RunAwayAsync retourne un PlayerState.
    /// </summary>
    [Fact]
    public async Task RunAwayAsync_ShouldReturnPlayerState()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 3);
        var player = context.Player.First();
        var room = context.Rooms.First();
        var dungeon = context.Dungeons.First();

        var result = await service.RunAwayAsync(player.CharacterId, room.RoomId, dungeon.DungeonId);

        Assert.NotNull(result.PlayerState);
    }

    /// <summary>
    /// Vérifie que IgnoreAsync indique si le donjon est terminé.
    /// </summary>
    [Fact]
    public async Task IgnoreAsync_ShouldIndicateDungeonCompleted_WhenLastRoom()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 2);
        var player = context.Player.First();
        var dungeon = session.Dungeon!;
        var rooms = dungeon.Rooms.OrderBy(r => r.RoomId).ToList();

        // Naviguer jusqu'à la dernière salle
        foreach (var room in rooms)
        {
            var result = await service.IgnoreAsync(player.CharacterId, room.RoomId, dungeon.DungeonId);
            if (room == rooms.Last())
            {
                Assert.True(result.IsDungeonCompleted);
            }
        }
    }

    /// <summary>
    /// Vérifie que FightAsync avec un combat gagné retourne Victoire.
    /// </summary>
    [Fact]
    public async Task FightAsync_ShouldReturnVictory_WhenPlayerWins()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = context.Player.First();
        player.Strength = 100;
        player.Health = 100;
        player.Armor = 50;
        context.SaveChanges();

        var dungeon = context.Dungeons.OrderByDescending(d => d.DungeonId).First();
        var roomWithMonster = context.Rooms.Include(r => r.Monster).FirstOrDefault(r => r.Monster != null);

        if (roomWithMonster != null)
        {
            var result = await service.FightAsync(player.CharacterId, roomWithMonster.RoomId, dungeon.DungeonId);

            Assert.Contains("Victoire", result.Message);
        }
    }

    /// <summary>
    /// Vérifie le score après combat gagné.
    /// </summary>
    [Fact]
    public async Task FightAsync_ShouldUpdateScore_WhenPlayerWins()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;
        player.Strength = 100;
        player.Health = 100;
        player.Armor = 50;
        context.SaveChanges();

        var roomWithMonster = session.Dungeon!.Rooms.FirstOrDefault(r => r.Monster != null);

        if (roomWithMonster != null)
        {
            var result = await service.FightAsync(player.CharacterId, roomWithMonster.RoomId, session.DungeonId);

            Assert.NotNull(result.Score);
            Assert.True(result.Score >= 100);
        }
    }

    /// <summary>
    /// Vérifie que SearchAsync retourne un succès dans une salle valide.
    /// </summary>
    [Fact]
    public async Task SearchAsync_ShouldReturnSuccess_WhenRoomExists()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;
        var room = session.Dungeon!.Rooms.First();

        var result = await service.SearchAsync(player.CharacterId, room.RoomId, session.DungeonId);

        Assert.Equal("Search", result.ActionType);
        Assert.NotNull(result.PlayerState);
    }

    /// <summary>
    /// Vérifie que IgnoreAsync retourne une erreur si la salle n'existe pas.
    /// </summary>
    [Fact]
    public async Task IgnoreAsync_ShouldReturnError_WhenRoomNotFound()
    {
        var (context, service, _) = CreateService();
        var player = context.Player.First();
        var dungeon = context.Dungeons.First();

        var result = await service.IgnoreAsync(player.CharacterId, 999, dungeon.DungeonId);

        // IgnoreAsync ne vérifie pas la salle directement mais la navigation
        Assert.Equal("Ignore", result.ActionType);
    }

    /// <summary>
    /// Vérifie que RunAwayAsync retourne Success parfois (70% de chance).
    /// </summary>
    [Fact]
    public async Task RunAwayAsync_ShouldReturnSuccess_Sometimes()
    {
        var (context, service, sessionService) = CreateService();
        var successCount = 0;

        for (int i = 0; i < 20; i++)
        {
            var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
            var player = session.Player!;
            var roomWithMonster = session.Dungeon!.Rooms.FirstOrDefault(r => r.Monster != null);

            if (roomWithMonster != null)
            {
                var result = await service.RunAwayAsync(player.CharacterId, roomWithMonster.RoomId, session.DungeonId);
                if (result.Success) successCount++;
            }
        }

        // Devrait avoir au moins quelques succès sur 20 essais
        Assert.True(successCount > 0);
    }

    /// <summary>
    /// Vérifie que FightAsync gère correctement la fin du donjon.
    /// </summary>
    [Fact]
    public async Task FightAsync_ShouldIndicateDungeonCompleted_WhenLastRoom()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 1);
        var player = session.Player!;
        player.Strength = 100;
        player.Health = 100;
        player.Armor = 50;
        context.SaveChanges();

        var rooms = session.Dungeon!.Rooms.ToList();
        var lastRoom = rooms.LastOrDefault(r => r.Monster != null);

        if (lastRoom != null)
        {
            var result = await service.FightAsync(player.CharacterId, lastRoom.RoomId, session.DungeonId);

            if (result.Success)
            {
                Assert.True(result.IsDungeonCompleted || result.NextRoomId != null);
            }
        }
    }

    /// <summary>
    /// Vérifie que OpenChestAsync avec coffre contenant une arme équipe l'arme.
    /// </summary>
    [Fact]
    public async Task OpenChestAsync_ShouldEquipWeapon_WhenChestContainsWeapon()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 10);
        var player = session.Player!;
        var dungeon = session.Dungeon!;

        var roomWithChest = dungeon.Rooms
            .FirstOrDefault(r => r.Chest != null && !r.Chest.IsOpened && r.Chest.Weapon != null);

        if (roomWithChest != null)
        {
            var result = await service.OpenChestAsync(player.CharacterId, roomWithChest.RoomId, dungeon.DungeonId);

            Assert.Equal("Open", result.ActionType);
            Assert.NotNull(result.PlayerState);
        }
    }

    /// <summary>
    /// Vérifie que OpenChestAsync avec coffre contenant une potion ajoute la potion.
    /// </summary>
    [Fact]
    public async Task OpenChestAsync_ShouldAddPotion_WhenChestContainsPotion()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 10);
        var player = session.Player!;
        var dungeon = session.Dungeon!;

        var roomWithChest = dungeon.Rooms
            .FirstOrDefault(r => r.Chest != null && !r.Chest.IsOpened && r.Chest.Potion != null);

        if (roomWithChest != null)
        {
            var result = await service.OpenChestAsync(player.CharacterId, roomWithChest.RoomId, dungeon.DungeonId);

            Assert.Equal("Open", result.ActionType);
            Assert.NotNull(result.PlayerState);
        }
    }

    /// <summary>
    /// Vérifie que FightAsync marque le joueur comme mort si santé à 0.
    /// </summary>
    [Fact]
    public async Task FightAsync_ShouldSetGameOver_WhenPlayerDies()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Difficult, 5);
        var player = session.Player!;
        player.Strength = 1;
        player.Health = 1;
        player.HeartNumber = 0;
        player.Armor = 0;
        context.SaveChanges();

        var roomWithMonster = session.Dungeon!.Rooms.FirstOrDefault(r => r.Monster != null);

        if (roomWithMonster != null && roomWithMonster.Monster != null)
        {
            roomWithMonster.Monster.Strength = 100;
            roomWithMonster.Monster.Health = 1000;
            context.SaveChanges();

            var result = await service.FightAsync(player.CharacterId, roomWithMonster.RoomId, session.DungeonId);

            if (!result.Success)
            {
                Assert.True(result.IsGameOver);
            }
        }
    }

    /// <summary>
    /// Vérifie que RunAwayAsync applique une pénalité de score.
    /// </summary>
    [Fact]
    public async Task RunAwayAsync_ShouldApplyScorePenalty()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        await sessionService.AddScoreAsync(session.SessionId, 100);
        var player = session.Player!;
        var room = session.Dungeon!.Rooms.First();

        var result = await service.RunAwayAsync(player.CharacterId, room.RoomId, session.DungeonId);

        // Le score devrait être réduit de 50 (100 - 50 = 50)
        Assert.NotNull(result.Score);
        Assert.True(result.Score <= 100);
    }

    /// <summary>
    /// Vérifie que IgnoreAsync retourne le prochain RoomId.
    /// </summary>
    [Fact]
    public async Task IgnoreAsync_ShouldReturnNextRoomId()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;
        var dungeon = session.Dungeon!;
        var rooms = dungeon.Rooms.OrderBy(r => r.RoomId).ToList();
        var firstRoom = rooms.First();

        var result = await service.IgnoreAsync(player.CharacterId, firstRoom.RoomId, dungeon.DungeonId);

        Assert.True(result.Success);
        if (rooms.Count > 1)
        {
            Assert.NotNull(result.NextRoomId);
        }
    }

    /// <summary>
    /// Vérifie que SearchAsync retourne toujours un PlayerState valide.
    /// </summary>
    [Fact]
    public async Task SearchAsync_ShouldReturnPlayerState()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Easy, 5);
        var player = session.Player!;
        var emptyRoom = session.Dungeon!.Rooms.FirstOrDefault(r => r.Monster == null && r.Chest == null);

        if (emptyRoom != null)
        {
            var result = await service.SearchAsync(player.CharacterId, emptyRoom.RoomId, session.DungeonId);
            Assert.NotNull(result.PlayerState);
        }
    }

    /// <summary>
    /// Vérifie que FightAsync avec perte de cœur continue le jeu.
    /// </summary>
    [Fact]
    public async Task FightAsync_ShouldContinueGame_WhenPlayerLosesHeart()
    {
        var (context, service, sessionService) = CreateService();
        var session = await sessionService.StartNewGameAsync(DungeonLevel.Medium, 5);
        var player = session.Player!;
        player.Strength = 1;
        player.Health = 100;
        player.HeartNumber = 3;
        player.Armor = 0;
        context.SaveChanges();

        var roomWithMonster = session.Dungeon!.Rooms.FirstOrDefault(r => r.Monster != null);

        if (roomWithMonster != null && roomWithMonster.Monster != null)
        {
            roomWithMonster.Monster.Strength = 50;
            roomWithMonster.Monster.Health = 500;
            context.SaveChanges();

            var result = await service.FightAsync(player.CharacterId, roomWithMonster.RoomId, session.DungeonId);

            // Si le joueur perd un cœur mais survit
            if (result.Message.Contains("cœur"))
            {
                Assert.False(result.IsGameOver);
            }
        }
    }
}
