using BlazorGame.GameService.Data;
using BlazorGame.SharedModels.DTOs;
using BlazorGame.SharedModels.Enums.Utils;
using BlazorGame.SharedModels.Models.Entities;
using BlazorGame.SharedModels.Models.Environment;
using BlazorGame.SharedModels.Models.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour gérer les actions du joueur dans les salles.
/// </summary>
public class ActionService
{
    private readonly GameDatabaseContext _context;
    private readonly PlayerService _playerService;
    private readonly FightService _fightService;
    private readonly GameSessionService _sessionService;
    private static readonly Random _random = new();

    /// <summary>
    /// Initialise le service avec les dépendances nécessaires.
    /// </summary>
    public ActionService(GameDatabaseContext context, PlayerService playerService, FightService fightService, GameSessionService sessionService)
    {
        _context = context;
        _playerService = playerService;
        _fightService = fightService;
        _sessionService = sessionService;
    }

    /// <summary>
    /// Exécute l'action "Fuir" pour un joueur dans une salle.
    /// </summary>
    public async Task<ActionResultDto> RunAwayAsync(int playerId, int roomId, int dungeonId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Joueur introuvable.",
                ActionType = "RunAway"
            };
        }

        var room = await GetRoomWithDetailsAsync(roomId);
        if (room == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Salle introuvable.",
                ActionType = "RunAway"
            };
        }

        // 70% de chance de fuir avec succès
        var escapeSuccess = _random.NextDouble() < 0.7;

        // Appliquer la pénalité de score pour fuite
        var session = await _sessionService.GetActiveSessionByPlayerIdAsync(playerId);
        int? currentScore = null;
        if (session != null)
        {
            currentScore = await _sessionService.ApplyRunAwayPenaltyAsync(session.SessionId);
        }

        if (escapeSuccess)
        {
            var nextRoomId = await GetNextRoomIdAsync(dungeonId, roomId);

            return new ActionResultDto
            {
                Success = true,
                Message = $"Vous avez réussi à fuir ! (-50 points, Score: {currentScore ?? 0})",
                ActionType = "RunAway",
                NextRoomId = nextRoomId,
                IsDungeonCompleted = nextRoomId == null,
                PlayerState = CreatePlayerSnapshot(player),
                Score = currentScore
            };
        }

        // Échec de la fuite - le monstre attaque
        double damageTaken = 0;
        if (room.Monster != null)
        {
            damageTaken = Math.Max(room.Monster.Damage - player.Armor, 5);
            await _playerService.UpdatePlayerHealthAsync(playerId, player.Health - damageTaken);
            player = await _playerService.GetPlayerByIdAsync(playerId);
        }

        var isAlive = await _playerService.IsPlayerAliveAsync(playerId);

        return new ActionResultDto
        {
            Success = false,
            Message = $"Vous n'avez pas réussi à fuir ! Le monstre vous inflige {damageTaken} dégâts.",
            ActionType = "RunAway",
            DamageTaken = damageTaken,
            IsGameOver = !isAlive,
            PlayerState = CreatePlayerSnapshot(player!)
        };
    }

    /// <summary>
    /// Exécute l'action "Chercher" dans une salle.
    /// </summary>
    public async Task<ActionResultDto> SearchAsync(int playerId, int roomId, int dungeonId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Joueur introuvable.",
                ActionType = "Search"
            };
        }

        var room = await GetRoomWithDetailsAsync(roomId);
        if (room == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Salle introuvable.",
                ActionType = "Search"
            };
        }

        // 40% de chance de trouver quelque chose
        var foundSomething = _random.NextDouble() < 0.4;
        var nextRoomId = await GetNextRoomIdAsync(dungeonId, roomId);

        if (foundSomething)
        {
            // 50% potion, 50% rien de spécial mais bonus
            if (_random.NextDouble() < 0.5)
            {
                var potionType = _random.NextDouble() < 0.5 ? PotionType.Health : PotionType.Strength;
                var potion = new Potion
                {
                    PotionId = await GetNextPotionIdAsync(),
                    Type = potionType
                };

                await _playerService.AddPotionAsync(playerId, potion);
                player = await _playerService.GetPlayerByIdAsync(playerId);

                return new ActionResultDto
                {
                    Success = true,
                    Message = $"Vous avez trouvé une {(potionType == PotionType.Health ? "potion de vie" : "potion de force")} !",
                    ActionType = "Search",
                    NextRoomId = nextRoomId,
                    IsDungeonCompleted = nextRoomId == null,
                    Reward = new { Type = "Potion", PotionType = potionType.ToString() },
                    PlayerState = CreatePlayerSnapshot(player!)
                };
            }
        }

        return new ActionResultDto
        {
            Success = true,
            Message = "Vous n'avez rien trouvé d'intéressant.",
            ActionType = "Search",
            NextRoomId = nextRoomId,
            IsDungeonCompleted = nextRoomId == null,
            PlayerState = CreatePlayerSnapshot(player)
        };
    }

    /// <summary>
    /// Exécute l'action "Ouvrir" un coffre.
    /// </summary>
    public async Task<ActionResultDto> OpenChestAsync(int playerId, int roomId, int dungeonId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Joueur introuvable.",
                ActionType = "Open"
            };
        }

        var room = await GetRoomWithDetailsAsync(roomId);
        if (room == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Salle introuvable.",
                ActionType = "Open"
            };
        }

        if (room.Chest == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Il n'y a pas de coffre dans cette salle.",
                ActionType = "Open",
                PlayerState = CreatePlayerSnapshot(player)
            };
        }

        if (room.Chest.IsOpened)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Le coffre a déjà été ouvert.",
                ActionType = "Open",
                PlayerState = CreatePlayerSnapshot(player)
            };
        }

        // Marquer le coffre comme ouvert
        room.Chest.IsOpened = true;
        await _context.SaveChangesAsync();

        // Ajouter des points pour l'ouverture du coffre
        var session = await _sessionService.GetActiveSessionByPlayerIdAsync(playerId);
        int? currentScore = null;
        if (session != null)
        {
            currentScore = await _sessionService.AddChestOpenScoreAsync(session.SessionId);
        }

        var nextRoomId = await GetNextRoomIdAsync(dungeonId, roomId);
        object? reward = null;
        string message;

        // 20% de chance que ce soit un piège
        if (_random.NextDouble() < 0.2)
        {
            var trapDamage = 15.0;
            await _playerService.UpdatePlayerHealthAsync(playerId, player.Health - trapDamage);
            player = await _playerService.GetPlayerByIdAsync(playerId);
            var isAlive = await _playerService.IsPlayerAliveAsync(playerId);

            return new ActionResultDto
            {
                Success = false,
                Message = $"C'était un piège ! Vous subissez {trapDamage} dégâts.",
                ActionType = "Open",
                DamageTaken = trapDamage,
                IsGameOver = !isAlive,
                NextRoomId = isAlive ? nextRoomId : null,
                PlayerState = CreatePlayerSnapshot(player!)
            };
        }

        // Coffre avec récompense
        if (room.Chest.Weapon != null)
        {
            await _playerService.EquipWeaponAsync(playerId, room.Chest.Weapon.WeaponId);
            player = await _playerService.GetPlayerByIdAsync(playerId);
            message = $"Vous avez trouvé une {room.Chest.Weapon.Type} ! Elle est maintenant équipée. (+25 points, Score: {currentScore ?? 0})";
            reward = new { Type = "Weapon", WeaponType = room.Chest.Weapon.Type.ToString(), Damage = room.Chest.Weapon.Damage };
        }
        else if (room.Chest.Potion != null)
        {
            await _playerService.AddPotionAsync(playerId, room.Chest.Potion);
            player = await _playerService.GetPlayerByIdAsync(playerId);
            message = $"Vous avez trouvé une {(room.Chest.Potion.Type == PotionType.Health ? "potion de vie" : "potion de force")} ! (+25 points, Score: {currentScore ?? 0})";
            reward = new { Type = "Potion", PotionType = room.Chest.Potion.Type.ToString() };
        }
        else
        {
            message = $"Le coffre était vide... (+25 points, Score: {currentScore ?? 0})";
        }

        return new ActionResultDto
        {
            Success = true,
            Message = message,
            ActionType = "Open",
            NextRoomId = nextRoomId,
            IsDungeonCompleted = nextRoomId == null,
            Reward = reward,
            PlayerState = CreatePlayerSnapshot(player!),
            Score = currentScore
        };
    }

    /// <summary>
    /// Exécute l'action "Ignorer" et passe à la salle suivante.
    /// </summary>
    public async Task<ActionResultDto> IgnoreAsync(int playerId, int roomId, int dungeonId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Joueur introuvable.",
                ActionType = "Ignore"
            };
        }

        var nextRoomId = await GetNextRoomIdAsync(dungeonId, roomId);

        return new ActionResultDto
        {
            Success = true,
            Message = "Vous passez à la salle suivante.",
            ActionType = "Ignore",
            NextRoomId = nextRoomId,
            IsDungeonCompleted = nextRoomId == null,
            PlayerState = CreatePlayerSnapshot(player)
        };
    }

    /// <summary>
    /// Exécute l'action "Combattre" un monstre.
    /// </summary>
    public async Task<ActionResultDto> FightAsync(int playerId, int roomId, int dungeonId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Joueur introuvable.",
                ActionType = "Fight"
            };
        }

        var room = await GetRoomWithDetailsAsync(roomId);
        if (room == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Salle introuvable.",
                ActionType = "Fight"
            };
        }

        if (room.Monster == null)
        {
            return new ActionResultDto
            {
                Success = false,
                Message = "Il n'y a pas de monstre à combattre dans cette salle.",
                ActionType = "Fight",
                PlayerState = CreatePlayerSnapshot(player)
            };
        }

        // Utiliser le FightService existant
        var fightResult = await _fightService.ExecuteFightAsync(playerId, room.Monster.CharacterId);

        player = await _playerService.GetPlayerByIdAsync(playerId);
        var isAlive = await _playerService.IsPlayerAliveAsync(playerId);
        var nextRoomId = isAlive ? await GetNextRoomIdAsync(dungeonId, roomId) : null;

        // Le joueur gagne si le monstre est mort
        var playerWon = fightResult.Result.Contains("Victoire");
        var lostHeart = fightResult.Result.Contains("cœur");

        // Gérer le score
        var session = await _sessionService.GetActiveSessionByPlayerIdAsync(playerId);
        int? currentScore = null;
        if (session != null)
        {
            if (playerWon)
            {
                currentScore = await _sessionService.AddMonsterKillScoreAsync(session.SessionId);
            }

            // Si le donjon est terminé et le joueur a gagné
            if (isAlive && nextRoomId == null)
            {
                await _sessionService.CompleteGameVictoryAsync(session.SessionId);
                session = await _sessionService.GetSessionByIdAsync(session.SessionId);
                currentScore = session?.Score;
            }
            else if (!isAlive)
            {
                await _sessionService.CompleteGameDefeatAsync(session.SessionId);
                session = await _sessionService.GetSessionByIdAsync(session.SessionId);
                currentScore = session?.Score;
            }
            else
            {
                currentScore = session.Score;
            }
        }

        string message;
        if (playerWon)
        {
            message = $"Victoire ! Vous avez vaincu le {room.Monster.Type}. (+100 points, Score: {currentScore ?? 0})";
        }
        else if (lostHeart)
        {
            message = $"Vous avez perdu un cœur face au {room.Monster.Type}, mais vous survivez !";
        }
        else
        {
            message = $"Défaite... Le {room.Monster.Type} vous a vaincu. (Score final: {currentScore ?? 0})";
        }

        return new ActionResultDto
        {
            Success = playerWon,
            Message = message,
            ActionType = "Fight",
            NextRoomId = nextRoomId,
            IsGameOver = !isAlive,
            IsDungeonCompleted = isAlive && nextRoomId == null,
            PlayerState = CreatePlayerSnapshot(player!),
            Score = currentScore
        };
    }

    /// <summary>
    /// Récupère une salle avec tous ses détails (monstre, coffre).
    /// </summary>
    private async Task<Room?> GetRoomWithDetailsAsync(int roomId)
    {
        return await _context.Rooms
            .Include(r => r.Monster)
            .Include(r => r.Chest)
                .ThenInclude(c => c!.Weapon)
            .Include(r => r.Chest)
                .ThenInclude(c => c!.Potion)
            .FirstOrDefaultAsync(r => r.RoomId == roomId);
    }

    /// <summary>
    /// Récupère l'ID de la salle suivante dans le donjon.
    /// </summary>
    private async Task<int?> GetNextRoomIdAsync(int dungeonId, int currentRoomId)
    {
        var dungeon = await _context.Dungeons
            .Include(d => d.Rooms)
            .FirstOrDefaultAsync(d => d.DungeonId == dungeonId);

        if (dungeon == null) return null;

        // Ordonner les salles par RoomId pour garantir l'ordre correct
        var orderedRooms = dungeon.Rooms.OrderBy(r => r.RoomId).ToList();

        var currentIndex = orderedRooms.FindIndex(r => r.RoomId == currentRoomId);
        if (currentIndex == -1 || currentIndex >= orderedRooms.Count - 1)
        {
            // Marquer le donjon comme terminé
            dungeon.IsCompleted = true;
            await _context.SaveChangesAsync();
            return null;
        }

        return orderedRooms[currentIndex + 1].RoomId;
    }

    /// <summary>
    /// Récupère le prochain ID de potion disponible.
    /// </summary>
    private async Task<int> GetNextPotionIdAsync()
    {
        return await _context.Potions.AnyAsync()
            ? await _context.Potions.MaxAsync(p => p.PotionId) + 1
            : 1;
    }

    /// <summary>
    /// Crée un snapshot de l'état du joueur.
    /// </summary>
    private static PlayerSnapshotDto CreatePlayerSnapshot(Player player)
    {
        return new PlayerSnapshotDto
        {
            CharacterId = player.CharacterId,
            Health = player.Health,
            Strength = player.Strength,
            Armor = player.Armor,
            HeartNumber = player.HeartNumber,
            Damage = player.Damage,
            WeaponType = player.Weapon?.Type.ToString(),
            Potions = player.Potions?.Select(p => new PotionDto
            {
                PotionId = p.PotionId,
                Type = p.Type.ToString(),
                Effect = p.Effect
            }).ToList()
        };
    }
}
