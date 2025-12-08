using BlazorGame.GameService.Data;
using BlazorGame.SharedModels.Constants;
using BlazorGame.SharedModels.Enums.Environment;
using BlazorGame.SharedModels.Models;
using BlazorGame.SharedModels.Models.Entities;
using BlazorGame.SharedModels.Models.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour gérer les sessions de jeu (parties).
/// </summary>
public class GameSessionService
{
    private readonly GameDatabaseContext _context;
    private readonly DungeonsService _dungeonsService;

    // Points de base pour différentes actions
    private const int PointsPerMonsterKill = 100;
    private const int PointsPerChestOpen = 25;
    private const int PointsPerRoomExplored = 10;
    private const int PenaltyForRunAway = -50;
    private const int BonusDungeonComplete = 500;

    /// <summary>
    /// Initialise le service avec le contexte et le service de donjons.
    /// </summary>
    public GameSessionService(GameDatabaseContext context, DungeonsService dungeonsService)
    {
        _context = context;
        _dungeonsService = dungeonsService;
    }

    /// <summary>
    /// Crée une nouvelle partie pour un joueur existant avec un nouveau donjon.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="dungeonLevel">Niveau de difficulté du donjon.</param>
    /// <param name="roomCount">Nombre de salles dans le donjon.</param>
    /// <returns>La session de jeu créée.</returns>
    public async Task<GameSession> StartNewGameAsync(int playerId, DungeonLevel dungeonLevel = DungeonLevel.Easy, int roomCount = 5)
    {
        // Récupérer le joueur existant
        var player = await _context.Player.FindAsync(playerId);
        if (player == null)
        {
            throw new ArgumentException($"Le joueur avec l'ID {playerId} n'existe pas.", nameof(playerId));
        }

        // Générer un nouveau donjon
        var dungeon = await _dungeonsService.GenerateRandomDungeonAsync(dungeonLevel, roomCount);

        // Créer la session de jeu
        var firstRoom = dungeon.Rooms.OrderBy(r => r.RoomId).FirstOrDefault();
        var session = new GameSession
        {
            PlayerId = player.CharacterId,
            Player = player,
            DungeonId = dungeon.DungeonId,
            Dungeon = dungeon,
            CurrentRoomId = firstRoom?.RoomId ?? 0,
            Score = 0,
            Status = GameSessionStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Récupère une session par son identifiant.
    /// </summary>
    public async Task<GameSession?> GetSessionByIdAsync(int sessionId)
    {
        return await _context.GameSessions
            .Include(s => s.Player)
            .Include(s => s.Dungeon)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
    }

    /// <summary>
    /// Récupère la session active d'un joueur.
    /// </summary>
    public async Task<GameSession?> GetActiveSessionByPlayerIdAsync(int playerId)
    {
        return await _context.GameSessions
            .Include(s => s.Player)
            .Include(s => s.Dungeon)
            .FirstOrDefaultAsync(s => s.PlayerId == playerId && s.Status == GameSessionStatus.InProgress);
    }

    /// <summary>
    /// Récupère toutes les sessions terminées (historique des scores).
    /// </summary>
    public async Task<IReadOnlyList<GameSession>> GetCompletedSessionsAsync()
    {
        return await _context.GameSessions
            .Where(s => s.Status == GameSessionStatus.Victory || s.Status == GameSessionStatus.Defeat)
            .OrderByDescending(s => s.Score)
            .ThenByDescending(s => s.UpdatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère toutes les sessions de jeu (en cours, terminées, abandonnées, sauvegardées).
    /// </summary>
    public async Task<IReadOnlyList<GameSession>> GetAllSessionsAsync()
    {
        return await _context.GameSessions
            .Include(s => s.Player)
            .Include(s => s.Dungeon)
            .OrderByDescending(s => s.UpdatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Ajoute des points au score de la session.
    /// </summary>
    public async Task<int> AddScoreAsync(int sessionId, int points)
    {
        var session = await _context.GameSessions.FindAsync(sessionId);
        if (session == null) return 0;

        session.Score += points;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return session.Score;
    }

    /// <summary>
    /// Ajoute des points pour avoir tué un monstre.
    /// </summary>
    public async Task<int> AddMonsterKillScoreAsync(int sessionId)
    {
        return await AddScoreAsync(sessionId, PointsPerMonsterKill);
    }

    /// <summary>
    /// Ajoute des points pour avoir ouvert un coffre.
    /// </summary>
    public async Task<int> AddChestOpenScoreAsync(int sessionId)
    {
        return await AddScoreAsync(sessionId, PointsPerChestOpen);
    }

    /// <summary>
    /// Ajoute des points pour avoir exploré une salle.
    /// </summary>
    public async Task<int> AddRoomExploredScoreAsync(int sessionId)
    {
        return await AddScoreAsync(sessionId, PointsPerRoomExplored);
    }

    /// <summary>
    /// Applique la pénalité pour avoir fui un combat.
    /// </summary>
    public async Task<int> ApplyRunAwayPenaltyAsync(int sessionId)
    {
        return await AddScoreAsync(sessionId, PenaltyForRunAway);
    }

    /// <summary>
    /// Met à jour la salle actuelle de la session.
    /// </summary>
    public async Task UpdateCurrentRoomAsync(int sessionId, int roomId)
    {
        var session = await _context.GameSessions.FindAsync(sessionId);
        if (session == null) return;

        session.CurrentRoomId = roomId;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Marque la session comme victoire et ajoute le bonus.
    /// </summary>
    public async Task<GameSession?> CompleteGameVictoryAsync(int sessionId)
    {
        var session = await _context.GameSessions.FindAsync(sessionId);
        if (session == null) return null;

        session.Score += BonusDungeonComplete;
        session.Status = GameSessionStatus.Victory;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Marque la session comme défaite.
    /// </summary>
    public async Task<GameSession?> CompleteGameDefeatAsync(int sessionId)
    {
        var session = await _context.GameSessions.FindAsync(sessionId);
        if (session == null) return null;

        session.Status = GameSessionStatus.Defeat;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Abandonne une partie en cours.
    /// </summary>
    public async Task<GameSession?> AbandonGameAsync(int sessionId)
    {
        var session = await _context.GameSessions.FindAsync(sessionId);
        if (session == null) return null;

        session.Status = GameSessionStatus.Abandoned;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Récupère le top 10 des meilleurs scores.
    /// </summary>
    public async Task<IReadOnlyList<GameSession>> GetTopScoresAsync(int count = 10)
    {
        return await _context.GameSessions
            .Where(s => s.Status == GameSessionStatus.Victory)
            .OrderByDescending(s => s.Score)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// Sauvegarde une partie en cours pour la reprendre plus tard.
    /// </summary>
    public async Task<GameSession?> SaveGameAsync(int sessionId, int currentRoomId)
    {
        var session = await _context.GameSessions.FindAsync(sessionId);
        if (session == null || session.Status != GameSessionStatus.InProgress) return null;

        session.CurrentRoomId = currentRoomId;
        session.Status = GameSessionStatus.Saved;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Reprend une partie sauvegardée.
    /// </summary>
    public async Task<GameSession?> ResumeGameAsync(int sessionId)
    {
        var session = await _context.GameSessions
            .Include(s => s.Player)
            .Include(s => s.Dungeon)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null || session.Status != GameSessionStatus.Saved) return null;

        session.Status = GameSessionStatus.InProgress;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Récupère toutes les parties sauvegardées.
    /// </summary>
    public async Task<IReadOnlyList<GameSession>> GetSavedGamesAsync()
    {
        return await _context.GameSessions
            .Include(s => s.Player)
            .Include(s => s.Dungeon)
            .Where(s => s.Status == GameSessionStatus.Saved)
            .OrderByDescending(s => s.UpdatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Supprime une partie sauvegardée.
    /// </summary>
    public async Task<bool> DeleteSavedGameAsync(int sessionId)
    {
        var session = await _context.GameSessions.FindAsync(sessionId);
        if (session == null || session.Status != GameSessionStatus.Saved) return false;

        _context.GameSessions.Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }
}
