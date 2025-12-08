using BlazorGame.GameService.Services;
using BlazorGame.SharedModels.Enums.Environment;
using BlazorGame.SharedModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.GameService.Controllers;

/// <summary>
/// Contrôleur pour visualiser toutes les données du jeu (Administration/Debugging).
/// Utilisé pour consulter l'état global du jeu via Postman/Swagger.
/// Accessible uniquement aux utilisateurs avec le rôle Admin.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AdminController : ControllerBase
{
    private readonly RoomsService _roomsService;
    private readonly PlayerService _playerService;
    private readonly MonstersService _monstersService;
    private readonly DungeonsService _dungeonsService;
    private readonly GameSessionService _sessionService;

    /// <summary>
    /// Initialise le contrôleur avec tous les services nécessaires.
    /// </summary>
    public AdminController(
        RoomsService roomsService,
        PlayerService playerService,
        MonstersService monstersService,
        DungeonsService dungeonsService,
        GameSessionService sessionService)
    {
        _roomsService = roomsService;
        _playerService = playerService;
        _monstersService = monstersService;
        _dungeonsService = dungeonsService;
        _sessionService = sessionService;
    }

    /// <summary>
    /// Récupère une vue d'ensemble complète de toutes les données du jeu.
    /// </summary>
    /// <returns>Objet contenant toutes les listes d'entités.</returns>
    [HttpGet("overview")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGameOverview()
    {
        var rooms = await _roomsService.GetAllRoomsAsync();
        var players = await _playerService.GetAllPlayersAsync();
        var monsters = await _monstersService.GetAllMonstersAsync();
        var dungeons = await _dungeonsService.GetAllDungeonsAsync();
        var completedSessions = await _sessionService.GetCompletedSessionsAsync();

        var overview = new
        {
            Statistics = new
            {
                TotalRooms = rooms.Count,
                TotalPlayers = players.Count,
                TotalMonsters = monsters.Count,
                TotalDungeons = dungeons.Count,
                TotalSessions = completedSessions.Count
            },
            Rooms = rooms,
            Players = players,
            Monsters = monsters,
            Dungeons = dungeons,
            Sessions = completedSessions
        };

        return Ok(overview);
    }

    /// <summary>
    /// Récupère la liste complète des salles avec leurs détails.
    /// </summary>
    /// <returns>Liste de toutes les salles.</returns>
    [HttpGet("rooms")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRooms()
    {
        var rooms = await _roomsService.GetAllRoomsAsync();
        return Ok(new
        {
            Count = rooms.Count,
            Data = rooms
        });
    }

    /// <summary>
    /// Récupère la liste complète des joueurs avec leurs statistiques.
    /// </summary>
    /// <returns>Liste de tous les joueurs.</returns>
    [HttpGet("players")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPlayers()
    {
        var players = await _playerService.GetAllPlayersAsync();
        return Ok(new
        {
            Count = players.Count,
            Data = players
        });
    }

    /// <summary>
    /// Récupère la liste complète des monstres.
    /// </summary>
    /// <returns>Liste de tous les monstres.</returns>
    [HttpGet("monsters")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMonsters()
    {
        var monsters = await _monstersService.GetAllMonstersAsync();
        return Ok(new
        {
            Count = monsters.Count,
            Data = monsters
        });
    }

    /// <summary>
    /// Récupère la liste complète des donjons.
    /// </summary>
    /// <returns>Liste de tous les donjons.</returns>
    [HttpGet("dungeons")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDungeons()
    {
        var dungeons = await _dungeonsService.GetAllDungeonsAsync();
        return Ok(new
        {
            Count = dungeons.Count,
            Data = dungeons
        });
    }

    /// <summary>
    /// Récupère la liste de toutes les sessions de jeu (en cours et terminées).
    /// </summary>
    /// <returns>Liste de toutes les sessions.</returns>
    [HttpGet("sessions")]
    [Authorize(Policy = "AdminOrPlayer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSessions()
    {
        var sessions = await _sessionService.GetAllSessionsAsync();
        return Ok(new
        {
            Count = sessions.Count,
            Data = sessions
        });
    }

    /// <summary>
    /// Récupère la liste des actions/événements disponibles dans le jeu.
    /// </summary>
    /// <returns>Liste des actions possibles avec leurs descriptions.</returns>
    [HttpGet("actions")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAvailableActions()
    {
        var actions = Enum.GetValues<AvailableActions>()
            .Select(action => new
            {
                Id = (int)action,
                Name = action.ToString(),
                DisplayName = action switch
                {
                    AvailableActions.Fight => "Combattre",
                    AvailableActions.RunAway => "Fuir",
                    AvailableActions.Search => "Chercher",
                    AvailableActions.Open => "Ouvrir",
                    AvailableActions.Ignore => "Ignorer",
                    _ => action.ToString()
                },
                Description = action switch
                {
                    AvailableActions.Fight => "Combattre le monstre présent dans la salle",
                    AvailableActions.RunAway => "Fuir la salle et passer à la suivante",
                    AvailableActions.Search => "Chercher des objets cachés dans la salle",
                    AvailableActions.Open => "Ouvrir un coffre présent dans la salle",
                    AvailableActions.Ignore => "Ignorer le contenu et passer à la salle suivante",
                    _ => "Action non définie"
                }
            })
            .ToList();

        return Ok(new
        {
            Count = actions.Count,
            Data = actions
        });
    }

    /// <summary>
    /// Récupère des statistiques détaillées sur le jeu.
    /// </summary>
    /// <returns>Statistiques globales du jeu.</returns>
    [HttpGet("statistics")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGameStatistics()
    {
        var rooms = await _roomsService.GetAllRoomsAsync();
        var players = await _playerService.GetAllPlayersAsync();
        var monsters = await _monstersService.GetAllMonstersAsync();
        var dungeons = await _dungeonsService.GetAllDungeonsAsync();
        var sessions = await _sessionService.GetAllSessionsAsync();

        var stats = new
        {
            EntityCounts = new
            {
                Rooms = rooms.Count,
                Players = players.Count,
                Monsters = monsters.Count,
                Dungeons = dungeons.Count,
                TotalSessions = sessions.Count
            },
            PlayerStatistics = new
            {
                AlivePlayers = players.Count(p => p.Health > 0),
                DeadPlayers = players.Count(p => p.Health <= 0),
                AverageHealth = players.Any() ? players.Average(p => p.Health) : 0,
                AverageStrength = players.Any() ? players.Average(p => p.Strength) : 0
            },
            MonsterStatistics = new
            {
                AliveMonsters = monsters.Count(m => m.Health > 0),
                DeadMonsters = monsters.Count(m => m.Health <= 0),
                AverageHealth = monsters.Any() ? monsters.Average(m => m.Health) : 0,
                AverageDamage = monsters.Any() ? monsters.Average(m => m.Damage) : 0
            },
            SessionStatistics = new
            {
                ActiveSessions = sessions.Count(s => s.Status == GameSessionStatus.InProgress),
                CompletedSessions = sessions.Count(s => s.Status == GameSessionStatus.Victory || s.Status == GameSessionStatus.Defeat),
                AbandonedSessions = sessions.Count(s => s.Status == GameSessionStatus.Abandoned),
                SavedSessions = sessions.Count(s => s.Status == GameSessionStatus.Saved),
                AverageScore = sessions.Any() ? sessions.Average(s => s.Score) : 0
            },
            RoomStatistics = new
            {
                RoomsWithMonsters = rooms.Count(r => r.MonsterId.HasValue),
                RoomsWithChests = rooms.Count(r => r.ChestId.HasValue),
                EmptyRooms = rooms.Count(r => !r.MonsterId.HasValue && !r.ChestId.HasValue)
            }
        };

        return Ok(stats);
    }

    // ==================== GESTION DES JOUEURS ====================

    /// <summary>
    /// Active ou désactive un joueur.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="isActive">True pour activer, False pour désactiver.</param>
    /// <returns>Le joueur mis à jour.</returns>
    [HttpPatch("players/{playerId}/toggle-active")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TogglePlayerActive(int playerId, [FromQuery] bool isActive)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        var result = await _playerService.SetPlayerActiveStatusAsync(playerId, isActive);
        if (!result)
        {
            return BadRequest(new { message = "Impossible de mettre à jour le statut du joueur." });
        }

        return Ok(new
        {
            message = isActive ? "Joueur activé avec succès." : "Joueur désactivé avec succès.",
            playerId,
            isActive
        });
    }

    /// <summary>
    /// Supprime un joueur (soft delete - désactivation).
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <returns>Confirmation de suppression.</returns>
    [HttpDelete("players/{playerId}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlayer(int playerId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        // Soft delete - on désactive le joueur
        var result = await _playerService.SetPlayerActiveStatusAsync(playerId, false);
        if (!result)
        {
            return BadRequest(new { message = "Impossible de supprimer le joueur." });
        }

        return Ok(new { message = "Joueur supprimé avec succès.", playerId });
    }

    // ==================== CLASSEMENTS ET SCORES ====================

    /// <summary>
    /// Récupère le classement général des joueurs par score total.
    /// </summary>
    /// <param name="top">Nombre de joueurs à afficher (par défaut 100).</param>
    /// <returns>Classement des joueurs.</returns>
    [HttpGet("leaderboard")]
    [Authorize(Policy = "AdminOrPlayer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaderboard([FromQuery] int top = 100)
    {
        var sessions = await _sessionService.GetAllSessionsAsync();

        var leaderboard = sessions
            .Where(s => s.Status == GameSessionStatus.Victory || s.Status == GameSessionStatus.Defeat)
            .GroupBy(s => s.PlayerId)
            .Select(g => new
            {
                PlayerId = g.Key,
                TotalScore = g.Sum(s => s.Score),
                GamesPlayed = g.Count(),
                Victories = g.Count(s => s.Status == GameSessionStatus.Victory),
                Defeats = g.Count(s => s.Status == GameSessionStatus.Defeat),
                BestScore = g.Max(s => s.Score),
                AverageScore = g.Average(s => s.Score),
                LastPlayed = g.Max(s => s.UpdatedAt)
            })
            .OrderByDescending(p => p.TotalScore)
            .Take(top)
            .ToList();

        return Ok(new
        {
            Count = leaderboard.Count,
            Data = leaderboard
        });
    }

    /// <summary>
    /// Récupère les statistiques détaillées d'un joueur.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <returns>Statistiques du joueur.</returns>
    [HttpGet("players/{playerId}/stats")]
    [Authorize(Policy = "AdminOrPlayer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlayerStats(int playerId)
    {
        var player = await _playerService.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return NotFound(new { message = $"Le joueur avec l'ID {playerId} n'existe pas." });
        }

        var sessions = await _sessionService.GetAllSessionsAsync();
        var playerSessions = sessions.Where(s => s.PlayerId == playerId).ToList();

        var stats = new
        {
            Player = player,
            Statistics = new
            {
                TotalGames = playerSessions.Count,
                GamesInProgress = playerSessions.Count(s => s.Status == GameSessionStatus.InProgress),
                Victories = playerSessions.Count(s => s.Status == GameSessionStatus.Victory),
                Defeats = playerSessions.Count(s => s.Status == GameSessionStatus.Defeat),
                Abandoned = playerSessions.Count(s => s.Status == GameSessionStatus.Abandoned),
                TotalScore = playerSessions.Sum(s => s.Score),
                BestScore = playerSessions.Any() ? playerSessions.Max(s => s.Score) : 0,
                AverageScore = playerSessions.Any() ? playerSessions.Average(s => s.Score) : 0,
                WinRate = playerSessions.Count > 0
                    ? (double)playerSessions.Count(s => s.Status == GameSessionStatus.Victory) / playerSessions.Count * 100
                    : 0,
                LastPlayed = playerSessions.Any() ? playerSessions.Max(s => s.UpdatedAt) : (DateTime?)null
            }
        };

        return Ok(stats);
    }

}
