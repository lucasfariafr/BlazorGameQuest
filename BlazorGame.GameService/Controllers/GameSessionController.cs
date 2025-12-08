using BlazorGame.GameService.Services;
using BlazorGame.GameService.Helpers;
using BlazorGame.SharedModels.DTOs;
using BlazorGame.SharedModels.Enums.Environment;
using BlazorGame.SharedModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.GameService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = "AdminOrPlayer")]
public class GameSessionController : ControllerBase
{
    private readonly GameSessionService _sessionService;
    private readonly PlayerService _playerService;

    public GameSessionController(GameSessionService sessionService, PlayerService playerService)
    {
        _sessionService = sessionService;
        _playerService = playerService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartNewGame([FromBody] StartGameRequestDto? request)
    {
        var userId = UserHelper.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Utilisateur non authentifié." });

        // Créer un NOUVEAU personnage pour chaque nouvelle partie
        var player = await _playerService.CreateNewPlayerForUserAsync(userId);

        var level = (request?.DifficultyLevel?.ToLower()) switch
        {
            "medium" => DungeonLevel.Medium,
            "difficult" or "hard" => DungeonLevel.Difficult,
            _ => DungeonLevel.Easy
        };

        var roomCount = request?.RoomCount ?? 5;
        if (roomCount < 1 || roomCount > 20)
            roomCount = 5;

        var session = await _sessionService.StartNewGameAsync(player.CharacterId, level, roomCount);
        var dto = await CreateSessionDto(session);

        return CreatedAtAction(nameof(GetSession), new { sessionId = session.SessionId }, dto);
    }

    [HttpGet("{sessionId:int}")]
    public async Task<IActionResult> GetSession(int sessionId)
    {
        var session = await _sessionService.GetSessionByIdAsync(sessionId);
        if (session == null)
            return NotFound(new { message = $"La session {sessionId} n'existe pas." });

        var dto = await CreateSessionDto(session);
        return Ok(dto);
    }

    [HttpGet("scores")]
    public async Task<IActionResult> GetScoreHistory()
    {
        var sessions = await _sessionService.GetTopScoresAsync(10);
        var history = sessions.Select(s => new ScoreHistoryDto
        {
            SessionId = s.SessionId,
            Score = s.Score,
            Status = s.Status.ToString(),
            PlayedAt = s.UpdatedAt
        }).ToList();

        return Ok(history);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetCompletedSessions()
    {
        var sessions = await _sessionService.GetCompletedSessionsAsync();
        var history = sessions.Select(s => new ScoreHistoryDto
        {
            SessionId = s.SessionId,
            Score = s.Score,
            Status = s.Status.ToString(),
            PlayedAt = s.UpdatedAt
        }).ToList();

        return Ok(history);
    }

    [HttpPost("{sessionId:int}/abandon")]
    public async Task<IActionResult> AbandonGame(int sessionId)
    {
        var session = await _sessionService.AbandonGameAsync(sessionId);
        if (session == null)
            return NotFound(new { message = $"La session {sessionId} n'existe pas." });

        return Ok(new { message = "Partie abandonnée.", score = session.Score });
    }

    [HttpPost("{sessionId:int}/save")]
    public async Task<IActionResult> SaveGame(int sessionId, [FromQuery] int currentRoomId)
    {
        var session = await _sessionService.SaveGameAsync(sessionId, currentRoomId);
        if (session == null)
            return NotFound(new { message = $"La session {sessionId} n'existe pas ou n'est pas en cours." });

        return Ok(new { message = "Partie sauvegardée.", sessionId = session.SessionId, score = session.Score });
    }

    [HttpGet("saved")]
    public async Task<IActionResult> GetSavedGames()
    {
        var sessions = await _sessionService.GetSavedGamesAsync();
        var result = new List<SavedGameDto>();

        foreach (var s in sessions)
        {
            var dto = await CreateSessionDto(s);
            result.Add(new SavedGameDto
            {
                SessionId = s.SessionId,
                Score = s.Score,
                CurrentRoomId = s.CurrentRoomId,
                DungeonId = s.DungeonId,
                PlayerId = s.PlayerId,
                SavedAt = s.UpdatedAt,
                PlayerState = dto.PlayerState
            });
        }

        return Ok(result);
    }

    [HttpPost("{sessionId:int}/resume")]
    public async Task<IActionResult> ResumeGame(int sessionId)
    {
        var session = await _sessionService.ResumeGameAsync(sessionId);
        if (session == null)
            return NotFound(new { message = $"La session {sessionId} n'existe pas ou n'est pas sauvegardée." });

        var dto = await CreateSessionDto(session);
        return Ok(dto);
    }

    [HttpDelete("{sessionId:int}")]
    public async Task<IActionResult> DeleteSavedGame(int sessionId)
    {
        var deleted = await _sessionService.DeleteSavedGameAsync(sessionId);
        if (!deleted)
            return NotFound(new { message = $"La session {sessionId} n'existe pas ou n'est pas sauvegardée." });

        return Ok(new { message = "Partie supprimée." });
    }

    private async Task<GameSessionDto> CreateSessionDto(GameSession session)
    {
        var player = await _playerService.GetPlayerByIdAsync(session.PlayerId);

        return new GameSessionDto
        {
            SessionId = session.SessionId,
            PlayerId = session.PlayerId,
            DungeonId = session.DungeonId,
            CurrentRoomId = session.CurrentRoomId,
            Score = session.Score,
            Status = session.Status.ToString(),
            CreatedAt = session.CreatedAt,
            PlayerState = player != null ? new PlayerSnapshotDto
            {
                CharacterId = player.CharacterId,
                Health = player.Health,
                Strength = player.Strength,
                Armor = player.Armor,
                Damage = player.Damage,
                HeartNumber = player.HeartNumber,
                WeaponType = player.Weapon?.Type.ToString(),
                Potions = player.Potions?.Select(p => new PotionDto
                {
                    PotionId = p.PotionId,
                    Type = p.Type.ToString(),
                    Effect = p.Effect
                }).ToList()
            } : null
        };
    }
}
