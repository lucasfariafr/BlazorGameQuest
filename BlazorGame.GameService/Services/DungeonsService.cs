using BlazorGame.GameService.Data;
using BlazorGame.SharedModels.Enums.Entities;
using BlazorGame.SharedModels.Enums.Environment;
using BlazorGame.SharedModels.Enums.Utils;
using BlazorGame.SharedModels.Models.Entities;
using BlazorGame.SharedModels.Models.Environment;
using BlazorGame.SharedModels.Models.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour gérer les donjons.
/// </summary>
public class DungeonsService
{
    private readonly GameDatabaseContext _context;
    private static readonly Random _random = new();

    private static readonly string[] MonsterDescriptions =
    [
        "Un {0} surgit de l'ombre. Que faites-vous ?",
        "Un {0} bloque le passage. Que faites-vous ?",
        "Un {0} féroce apparaît devant vous !",
        "Vous entendez un grognement... Un {0} vous attaque !"
    ];

    private static readonly string[] ChestDescriptions =
    [
        "Un coffre mystérieux brille dans la pénombre.",
        "Vous découvrez un coffre ancien.",
        "Une malle poussiéreuse attire votre attention.",
        "Un coffre orné de joyaux vous attend."
    ];

    private static readonly string[] EmptyRoomDescriptions =
    [
        "Une salle vide et silencieuse.",
        "Rien de particulier ici... ou presque.",
        "La pièce semble abandonnée depuis longtemps.",
        "Un couloir sombre s'étend devant vous."
    ];

    /// <summary>
    /// Initialise le service avec le contexte de la base de données.
    /// </summary>
    /// <param name="context">Le contexte de la base de données du jeu.</param>
    public DungeonsService(GameDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère tous les donjons.
    /// </summary>
    /// <returns>Liste de tous les donjons.</returns>
    public async Task<IReadOnlyList<Dungeon>> GetAllDungeonsAsync()
    {
        return await _context.Dungeons.ToListAsync();
    }

    /// <summary>
    /// Récupère un donjon par son identifiant.
    /// </summary>
    /// <param name="dungeonId">Identifiant du donjon.</param>
    /// <returns>Le donjon ou null s'il n'existe pas.</returns>
    public async Task<Dungeon?> GetDungeonByIdAsync(int dungeonId)
    {
        return await _context.Dungeons.FindAsync(dungeonId);
    }

    /// <summary>
    /// Génère un donjon aléatoire avec des salles, monstres et coffres.
    /// </summary>
    /// <param name="level">Niveau de difficulté du donjon.</param>
    /// <param name="roomCount">Nombre de salles à générer.</param>
    /// <returns>Le donjon généré.</returns>
    public async Task<Dungeon> GenerateRandomDungeonAsync(DungeonLevel level, int roomCount = 5)
    {
        var rooms = new List<Room>();
        var monsterTypes = Enum.GetValues<MonsterType>();
        var weaponTypes = Enum.GetValues<WeaponType>();
        var potionTypes = Enum.GetValues<PotionType>();

        // Obtenir le prochain ID disponible pour le donjon
        var nextDungeonId = await _context.Dungeons.AnyAsync()
            ? await _context.Dungeons.MaxAsync(d => d.DungeonId) + 1
            : 1;

        // Compteurs pour les IDs
        var nextRoomId = await _context.Rooms.AnyAsync()
            ? await _context.Rooms.MaxAsync(r => r.RoomId) + 1
            : 1;
        var nextMonsterId = await _context.Monsters.AnyAsync()
            ? await _context.Monsters.MaxAsync(m => m.CharacterId) + 1
            : 1;
        var nextChestId = await _context.Chests.AnyAsync()
            ? await _context.Chests.MaxAsync(c => c.ChestId) + 1
            : 1;
        var nextWeaponId = await _context.Weapons.AnyAsync()
            ? await _context.Weapons.MaxAsync(w => w.WeaponId) + 1
            : 1;
        var nextPotionId = await _context.Potions.AnyAsync()
            ? await _context.Potions.MaxAsync(p => p.PotionId) + 1
            : 1;

        // Générer une distribution équilibrée des salles
        // Au moins 50% de monstres, 30% de coffres, 20% de salles vides
        var roomTypes = new List<int>();
        var monsterCount = Math.Max(1, roomCount / 2); // Au moins 1 monstre, 50% du total
        var chestCount = Math.Max(1, roomCount / 3);   // Au moins 1 coffre, ~30%
        var emptyCount = roomCount - monsterCount - chestCount;

        for (int i = 0; i < monsterCount; i++) roomTypes.Add(0); // Monstre
        for (int i = 0; i < chestCount; i++) roomTypes.Add(1);   // Coffre
        for (int i = 0; i < emptyCount; i++) roomTypes.Add(2);   // Vide

        // Mélanger la liste pour un ordre aléatoire
        for (int i = roomTypes.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (roomTypes[i], roomTypes[j]) = (roomTypes[j], roomTypes[i]);
        }

        for (int i = 0; i < roomCount; i++)
        {
            var roomType = roomTypes[i];

            var room = roomType switch
            {
                0 => CreateMonsterRoom(ref nextRoomId, ref nextMonsterId, monsterTypes, level),
                1 => CreateChestRoom(ref nextRoomId, ref nextChestId, ref nextWeaponId, ref nextPotionId, weaponTypes, potionTypes),
                _ => CreateEmptyRoom(ref nextRoomId)
            };

            rooms.Add(room);
        }

        var dungeon = new Dungeon
        {
            DungeonId = nextDungeonId,
            DifficultyLevel = level,
            Rooms = rooms,
            IsCompleted = false
        };

        _context.Dungeons.Add(dungeon);
        await _context.SaveChangesAsync();

        return dungeon;
    }

    /// <summary>
    /// Crée une salle avec un monstre.
    /// </summary>
    private Room CreateMonsterRoom(ref int roomId, ref int monsterId, MonsterType[] monsterTypes, DungeonLevel level)
    {
        var monsterType = monsterTypes[_random.Next(monsterTypes.Length)];
        var description = string.Format(
            MonsterDescriptions[_random.Next(MonsterDescriptions.Length)],
            monsterType);

        // Stats du monstre selon le niveau de difficulté
        var (strength, armor) = GetMonsterStats(monsterType, level);

        var currentMonsterId = monsterId++;
        var monster = new Monster
        {
            CharacterId = currentMonsterId,
            Type = monsterType,
            Strength = strength,
            Armor = armor
        };

        return new Room
        {
            RoomId = roomId++,
            Description = description,
            Actions = [AvailableActions.Fight, AvailableActions.RunAway, AvailableActions.Search],
            MonsterId = currentMonsterId,
            Monster = monster,
            ChestId = null,
            Chest = null
        };
    }

    /// <summary>
    /// Crée une salle avec un coffre.
    /// </summary>
    private Room CreateChestRoom(ref int roomId, ref int chestId, ref int weaponId, ref int potionId, WeaponType[] weaponTypes, PotionType[] potionTypes)
    {
        var description = ChestDescriptions[_random.Next(ChestDescriptions.Length)];

        // 50% chance d'avoir une arme, 50% chance d'avoir une potion
        Weapon? weapon = null;
        Potion? potion = null;

        if (_random.NextDouble() < 0.5)
        {
            weapon = new Weapon
            {
                WeaponId = weaponId++,
                Type = weaponTypes[_random.Next(weaponTypes.Length)]
            };
        }
        else
        {
            potion = new Potion
            {
                PotionId = potionId++,
                Type = potionTypes[_random.Next(potionTypes.Length)]
            };
        }

        var currentChestId = chestId++;
        var chest = new Chest
        {
            ChestId = currentChestId,
            IsOpened = false,
            Weapon = weapon,
            Potion = potion
        };

        return new Room
        {
            RoomId = roomId++,
            Description = description,
            Actions = [AvailableActions.Open, AvailableActions.Ignore],
            MonsterId = null,
            Monster = null,
            ChestId = currentChestId,
            Chest = chest
        };
    }

    /// <summary>
    /// Crée une salle vide.
    /// </summary>
    private Room CreateEmptyRoom(ref int roomId)
    {
        var description = EmptyRoomDescriptions[_random.Next(EmptyRoomDescriptions.Length)];

        return new Room
        {
            RoomId = roomId++,
            Description = description,
            Actions = [AvailableActions.Search, AvailableActions.Ignore],
            MonsterId = null,
            Monster = null,
            ChestId = null,
            Chest = null
        };
    }

    /// <summary>
    /// Retourne les stats du monstre selon son type et le niveau de difficulté.
    /// </summary>
    private static (double strength, double armor) GetMonsterStats(MonsterType type, DungeonLevel level)
    {
        var difficultyMultiplier = level switch
        {
            DungeonLevel.Easy => 1.0,
            DungeonLevel.Medium => 1.5,
            DungeonLevel.Difficult => 2.0,
            _ => 1.0
        };

        return type switch
        {
            MonsterType.Goblin => (5 * difficultyMultiplier, 2 * difficultyMultiplier),
            MonsterType.Ogre => (15 * difficultyMultiplier, 8 * difficultyMultiplier),
            MonsterType.Zombie => (10 * difficultyMultiplier, 5 * difficultyMultiplier),
            _ => (5 * difficultyMultiplier, 2 * difficultyMultiplier)
        };
    }
}
