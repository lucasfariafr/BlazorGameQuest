namespace BlazorGame.GameService.Data;

public static class DatabaseInitializer
{
    public static void Initialize(GameDatabaseContext context)
    {
        if (context.Dungeons.Any())
        {
            return;
        }

        var sword = new Weapon
        {
            WeaponId = 1,
            Type = WeaponType.Sword
        };

        var bow = new Weapon
        {
            WeaponId = 2,
            Type = WeaponType.Bow
        };

        var wand = new Weapon
        {
            WeaponId = 3,
            Type = WeaponType.Wand
        };

        var healthPotion = new Potion
        {
            PotionId = 1,
            Type = PotionType.Health
        };

        var strengthPotion = new Potion
        {
            PotionId = 2,
            Type = PotionType.Strength
        };
        
        var player = new Player
        {
            CharacterId = 1,
            Strength = 23,
            Armor = 5,
            Weapon = sword,
            Potions = new List<Potion> { healthPotion, strengthPotion }
        };

        var zombie = new Monster
        {
            CharacterId = 2,
            Type = MonsterType.Zombie,
            Strength = 12,
            Armor = 6,
            Weapon = sword
        };

        var goblin = new Monster
        {
            CharacterId = 3,
            Type = MonsterType.Goblin,
            Strength = 8,
            Armor = 4,
            Weapon = bow
        };

        var chest1 = new Chest
        {
            ChestId = 1,
            IsOpened = false,
            Potion = healthPotion
        };

        var room1 = new Room
        {
            RoomId = 1,
            Description = $"Un {zombie.Type.ToString().ToLower()} apparaît. Que faites-vous ?",
            Actions = new List<AvailableActions>
            {
                AvailableActions.Fight,
                AvailableActions.RunAway,
                AvailableActions.Search
            },
            Monster = zombie
        };

        var room2 = new Room
        {
            RoomId = 2,
            Description = "Un coffre mystérieux !",
            Actions = new List<AvailableActions>
            {
                AvailableActions.Open,
                AvailableActions.Ignore
            },
            Chest = chest1
        };

        var dungeon = new Dungeon
        {
            DungeonId = 1,
            DifficultyLevel = DungeonLevel.Easy,
            IsCompleted = false,
            Rooms = new List<Room> { room1, room2 }
        };

        context.AddRange(
        [
            sword, bow, wand,
            healthPotion, strengthPotion,
            player,
            zombie, goblin,
            chest1,
            room1, room2,
            dungeon
        ]);
        
        context.SaveChanges();
    }
}
