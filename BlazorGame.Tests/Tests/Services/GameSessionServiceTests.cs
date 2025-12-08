using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using BlazorGame.SharedModels.Enums.Environment;
using BlazorGame.SharedModels.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Tests.Tests;

/// <summary>
/// Tests unitaires pour vérifier le bon fonctionnement du GameSessionService.
/// </summary>
public class GameSessionServiceTests
{
    /// <summary>
    /// Crée un contexte EF Core en mémoire et initialise la base de données.
    /// </summary>
    private static GameDatabaseContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDatabaseContext>()
            .UseInMemoryDatabase($"GameDb_SessionTest_{Guid.NewGuid()}")
            .Options;

        var context = new GameDatabaseContext(options);
        DatabaseInitializer.Initialize(context);
        return context;
    }

    /// <summary>
    /// Crée le service GameSessionService avec ses dépendances.
    /// </summary>
    private static (GameDatabaseContext Context, GameSessionService Service) CreateService()
    {
        var context = CreateInMemoryContext();
        var dungeonsService = new DungeonsService(context);
        var service = new GameSessionService(context, dungeonsService);
        return (context, service);
    }

    /// <summary>
    /// Vérifie que StartNewGameAsync crée une nouvelle session de jeu.
    /// </summary>
    [Fact]
    public async Task StartNewGameAsync_ShouldCreateNewSession()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();

        var session = await service.StartNewGameAsync(player.CharacterId, DungeonLevel.Easy, 3);

        Assert.NotNull(session);
        Assert.True(session.SessionId > 0);
        Assert.Equal(GameSessionStatus.InProgress, session.Status);
        Assert.Equal(0, session.Score);
        Assert.NotNull(session.Player);
        Assert.NotNull(session.Dungeon);
    }

    /// <summary>
    /// Vérifie que GetSessionByIdAsync retourne la session correcte.
    /// </summary>
    [Fact]
    public async Task GetSessionByIdAsync_ShouldReturnSession_WhenExists()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var found = await service.GetSessionByIdAsync(session.SessionId);

        Assert.NotNull(found);
        Assert.Equal(session.SessionId, found.SessionId);
    }

    /// <summary>
    /// Vérifie que GetSessionByIdAsync retourne null si la session n'existe pas.
    /// </summary>
    [Fact]
    public async Task GetSessionByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var (_, service) = CreateService();

        var found = await service.GetSessionByIdAsync(999);

        Assert.Null(found);
    }

    /// <summary>
    /// Vérifie que AddScoreAsync ajoute correctement les points.
    /// </summary>
    [Fact]
    public async Task AddScoreAsync_ShouldAddPoints()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var newScore = await service.AddScoreAsync(session.SessionId, 100);

        Assert.Equal(100, newScore);
    }

    /// <summary>
    /// Vérifie que AddMonsterKillScoreAsync ajoute 100 points.
    /// </summary>
    [Fact]
    public async Task AddMonsterKillScoreAsync_ShouldAdd100Points()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var newScore = await service.AddMonsterKillScoreAsync(session.SessionId);

        Assert.Equal(100, newScore);
    }

    /// <summary>
    /// Vérifie que AddChestOpenScoreAsync ajoute 25 points.
    /// </summary>
    [Fact]
    public async Task AddChestOpenScoreAsync_ShouldAdd25Points()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var newScore = await service.AddChestOpenScoreAsync(session.SessionId);

        Assert.Equal(25, newScore);
    }

    /// <summary>
    /// Vérifie que ApplyRunAwayPenaltyAsync retire 50 points.
    /// </summary>
    [Fact]
    public async Task ApplyRunAwayPenaltyAsync_ShouldSubtract50Points()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);
        await service.AddScoreAsync(session.SessionId, 100);

        var newScore = await service.ApplyRunAwayPenaltyAsync(session.SessionId);

        Assert.Equal(50, newScore);
    }

    /// <summary>
    /// Vérifie que SaveGameAsync sauvegarde correctement la partie.
    /// </summary>
    [Fact]
    public async Task SaveGameAsync_ShouldSaveGame()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var saved = await service.SaveGameAsync(session.SessionId, 5);

        Assert.NotNull(saved);
        Assert.Equal(GameSessionStatus.Saved, saved.Status);
        Assert.Equal(5, saved.CurrentRoomId);
    }

    /// <summary>
    /// Vérifie que SaveGameAsync retourne null si la session n'existe pas.
    /// </summary>
    [Fact]
    public async Task SaveGameAsync_ShouldReturnNull_WhenSessionNotExists()
    {
        var (_, service) = CreateService();

        var saved = await service.SaveGameAsync(999, 5);

        Assert.Null(saved);
    }

    /// <summary>
    /// Vérifie que ResumeGameAsync reprend correctement une partie sauvegardée.
    /// </summary>
    [Fact]
    public async Task ResumeGameAsync_ShouldResumeGame()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);
        await service.SaveGameAsync(session.SessionId, 5);

        var resumed = await service.ResumeGameAsync(session.SessionId);

        Assert.NotNull(resumed);
        Assert.Equal(GameSessionStatus.InProgress, resumed.Status);
    }

    /// <summary>
    /// Vérifie que ResumeGameAsync retourne null si la partie n'est pas sauvegardée.
    /// </summary>
    [Fact]
    public async Task ResumeGameAsync_ShouldReturnNull_WhenNotSaved()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var resumed = await service.ResumeGameAsync(session.SessionId);

        Assert.Null(resumed);
    }

    /// <summary>
    /// Vérifie que GetSavedGamesAsync retourne les parties sauvegardées.
    /// </summary>
    [Fact]
    public async Task GetSavedGamesAsync_ShouldReturnSavedGames()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session1 = await service.StartNewGameAsync(player.CharacterId);
        var session2 = await service.StartNewGameAsync(player.CharacterId);
        await service.SaveGameAsync(session1.SessionId, 5);
        await service.SaveGameAsync(session2.SessionId, 3);

        var savedGames = await service.GetSavedGamesAsync();

        Assert.Equal(2, savedGames.Count);
    }

    /// <summary>
    /// Vérifie que DeleteSavedGameAsync supprime la partie sauvegardée.
    /// </summary>
    [Fact]
    public async Task DeleteSavedGameAsync_ShouldDeleteSavedGame()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);
        await service.SaveGameAsync(session.SessionId, 5);

        var deleted = await service.DeleteSavedGameAsync(session.SessionId);

        Assert.True(deleted);
        var found = await service.GetSessionByIdAsync(session.SessionId);
        Assert.Null(found);
    }

    /// <summary>
    /// Vérifie que DeleteSavedGameAsync retourne false si la partie n'est pas sauvegardée.
    /// </summary>
    [Fact]
    public async Task DeleteSavedGameAsync_ShouldReturnFalse_WhenNotSaved()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var deleted = await service.DeleteSavedGameAsync(session.SessionId);

        Assert.False(deleted);
    }

    /// <summary>
    /// Vérifie que CompleteGameVictoryAsync marque la session comme victoire.
    /// </summary>
    [Fact]
    public async Task CompleteGameVictoryAsync_ShouldMarkAsVictory()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var completed = await service.CompleteGameVictoryAsync(session.SessionId);

        Assert.NotNull(completed);
        Assert.Equal(GameSessionStatus.Victory, completed.Status);
        Assert.Equal(500, completed.Score); // Bonus de victoire
    }

    /// <summary>
    /// Vérifie que CompleteGameDefeatAsync marque la session comme défaite.
    /// </summary>
    [Fact]
    public async Task CompleteGameDefeatAsync_ShouldMarkAsDefeat()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var completed = await service.CompleteGameDefeatAsync(session.SessionId);

        Assert.NotNull(completed);
        Assert.Equal(GameSessionStatus.Defeat, completed.Status);
    }

    /// <summary>
    /// Vérifie que AbandonGameAsync marque la session comme abandonnée.
    /// </summary>
    [Fact]
    public async Task AbandonGameAsync_ShouldMarkAsAbandoned()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var abandoned = await service.AbandonGameAsync(session.SessionId);

        Assert.NotNull(abandoned);
        Assert.Equal(GameSessionStatus.Abandoned, abandoned.Status);
    }

    /// <summary>
    /// Vérifie que GetTopScoresAsync retourne les meilleurs scores.
    /// </summary>
    [Fact]
    public async Task GetTopScoresAsync_ShouldReturnTopScores()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session1 = await service.StartNewGameAsync(player.CharacterId);
        var session2 = await service.StartNewGameAsync(player.CharacterId);
        await service.AddScoreAsync(session1.SessionId, 200);
        await service.AddScoreAsync(session2.SessionId, 100);
        await service.CompleteGameVictoryAsync(session1.SessionId);
        await service.CompleteGameVictoryAsync(session2.SessionId);

        var topScores = await service.GetTopScoresAsync(10);

        Assert.Equal(2, topScores.Count);
        Assert.True(topScores[0].Score >= topScores[1].Score);
    }

    /// <summary>
    /// Vérifie que GetCompletedSessionsAsync retourne les sessions terminées.
    /// </summary>
    [Fact]
    public async Task GetCompletedSessionsAsync_ShouldReturnCompletedSessions()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session1 = await service.StartNewGameAsync(player.CharacterId);
        var session2 = await service.StartNewGameAsync(player.CharacterId);
        await service.CompleteGameVictoryAsync(session1.SessionId);
        await service.CompleteGameDefeatAsync(session2.SessionId);

        var completed = await service.GetCompletedSessionsAsync();

        Assert.Equal(2, completed.Count);
    }

    /// <summary>
    /// Vérifie que UpdateCurrentRoomAsync met à jour la salle actuelle.
    /// </summary>
    [Fact]
    public async Task UpdateCurrentRoomAsync_ShouldUpdateCurrentRoom()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        await service.UpdateCurrentRoomAsync(session.SessionId, 10);

        var updated = await service.GetSessionByIdAsync(session.SessionId);
        Assert.NotNull(updated);
        Assert.Equal(10, updated.CurrentRoomId);
    }

    /// <summary>
    /// Vérifie que AddRoomExploredScoreAsync ajoute 10 points.
    /// </summary>
    [Fact]
    public async Task AddRoomExploredScoreAsync_ShouldAdd10Points()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var newScore = await service.AddRoomExploredScoreAsync(session.SessionId);

        Assert.Equal(10, newScore);
    }

    /// <summary>
    /// Vérifie que GetActiveSessionByPlayerIdAsync retourne la session active.
    /// </summary>
    [Fact]
    public async Task GetActiveSessionByPlayerIdAsync_ShouldReturnActiveSession()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var active = await service.GetActiveSessionByPlayerIdAsync(session.PlayerId);

        Assert.NotNull(active);
        Assert.Equal(session.SessionId, active.SessionId);
    }

    /// <summary>
    /// Vérifie que GetActiveSessionByPlayerIdAsync retourne null si aucune session active.
    /// </summary>
    [Fact]
    public async Task GetActiveSessionByPlayerIdAsync_ShouldReturnNull_WhenNoActiveSession()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);
        await service.CompleteGameVictoryAsync(session.SessionId);

        var active = await service.GetActiveSessionByPlayerIdAsync(session.PlayerId);

        Assert.Null(active);
    }

    /// <summary>
    /// Vérifie que AddScoreAsync retourne 0 si la session n'existe pas.
    /// </summary>
    [Fact]
    public async Task AddScoreAsync_ShouldReturnZero_WhenSessionNotExists()
    {
        var (_, service) = CreateService();

        var score = await service.AddScoreAsync(999, 100);

        Assert.Equal(0, score);
    }

    /// <summary>
    /// Vérifie que CompleteGameVictoryAsync retourne null si la session n'existe pas.
    /// </summary>
    [Fact]
    public async Task CompleteGameVictoryAsync_ShouldReturnNull_WhenSessionNotExists()
    {
        var (_, service) = CreateService();

        var result = await service.CompleteGameVictoryAsync(999);

        Assert.Null(result);
    }

    /// <summary>
    /// Vérifie que CompleteGameDefeatAsync retourne null si la session n'existe pas.
    /// </summary>
    [Fact]
    public async Task CompleteGameDefeatAsync_ShouldReturnNull_WhenSessionNotExists()
    {
        var (_, service) = CreateService();

        var result = await service.CompleteGameDefeatAsync(999);

        Assert.Null(result);
    }

    /// <summary>
    /// Vérifie que AbandonGameAsync retourne null si la session n'existe pas.
    /// </summary>
    [Fact]
    public async Task AbandonGameAsync_ShouldReturnNull_WhenSessionNotExists()
    {
        var (_, service) = CreateService();

        var result = await service.AbandonGameAsync(999);

        Assert.Null(result);
    }

    /// <summary>
    /// Vérifie que le score cumulé est correct après plusieurs actions.
    /// </summary>
    [Fact]
    public async Task Score_ShouldAccumulateCorrectly()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        await service.AddMonsterKillScoreAsync(session.SessionId); // +100
        await service.AddChestOpenScoreAsync(session.SessionId);    // +25
        await service.AddRoomExploredScoreAsync(session.SessionId); // +10

        var updated = await service.GetSessionByIdAsync(session.SessionId);
        Assert.NotNull(updated);
        Assert.Equal(135, updated.Score);
    }

    /// <summary>
    /// Vérifie que le score peut être négatif après fuite.
    /// </summary>
    [Fact]
    public async Task Score_CanBeNegative_AfterRunAway()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        await service.ApplyRunAwayPenaltyAsync(session.SessionId);

        var updated = await service.GetSessionByIdAsync(session.SessionId);
        Assert.NotNull(updated);
        Assert.Equal(-50, updated.Score);
    }

    /// <summary>
    /// Vérifie que StartNewGameAsync crée une session avec niveau Medium.
    /// </summary>
    [Fact]
    public async Task StartNewGameAsync_ShouldCreateSession_WithMediumLevel()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();

        var session = await service.StartNewGameAsync(player.CharacterId, DungeonLevel.Medium, 3);

        Assert.NotNull(session);
        Assert.Equal(DungeonLevel.Medium, session.Dungeon!.DifficultyLevel);
    }

    /// <summary>
    /// Vérifie que StartNewGameAsync crée une session avec niveau Difficult.
    /// </summary>
    [Fact]
    public async Task StartNewGameAsync_ShouldCreateSession_WithDifficultLevel()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();

        var session = await service.StartNewGameAsync(player.CharacterId, DungeonLevel.Difficult, 3);

        Assert.NotNull(session);
        Assert.Equal(DungeonLevel.Difficult, session.Dungeon!.DifficultyLevel);
    }

    /// <summary>
    /// Vérifie que GetSessionByIdAsync retourne le joueur inclus.
    /// </summary>
    [Fact]
    public async Task GetSessionByIdAsync_ShouldIncludePlayer()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var found = await service.GetSessionByIdAsync(session.SessionId);

        Assert.NotNull(found);
        Assert.NotNull(found.Player);
        Assert.True(found.Player.CharacterId > 0);
    }

    /// <summary>
    /// Vérifie que GetSessionByIdAsync retourne le donjon inclus.
    /// </summary>
    [Fact]
    public async Task GetSessionByIdAsync_ShouldIncludeDungeon()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var found = await service.GetSessionByIdAsync(session.SessionId);

        Assert.NotNull(found);
        Assert.NotNull(found.Dungeon);
    }

    /// <summary>
    /// Vérifie que UpdateCurrentRoomAsync ne fait rien si session inexistante.
    /// </summary>
    [Fact]
    public async Task UpdateCurrentRoomAsync_ShouldDoNothing_WhenSessionNotExists()
    {
        var (_, service) = CreateService();

        // Ne devrait pas lever d'exception
        await service.UpdateCurrentRoomAsync(999, 10);

        // Vérifie que rien n'a été créé
        var session = await service.GetSessionByIdAsync(999);
        Assert.Null(session);
    }

    /// <summary>
    /// Vérifie que les sessions en cours ne sont pas retournées par GetCompletedSessionsAsync.
    /// </summary>
    [Fact]
    public async Task GetCompletedSessionsAsync_ShouldNotIncludeInProgressSessions()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        var completed = await service.GetCompletedSessionsAsync();

        Assert.DoesNotContain(completed, s => s.SessionId == session.SessionId);
    }

    /// <summary>
    /// Vérifie que GetTopScoresAsync retourne les scores triés.
    /// </summary>
    [Fact]
    public async Task GetTopScoresAsync_ShouldReturnScoresInDescendingOrder()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session1 = await service.StartNewGameAsync(player.CharacterId);
        var session2 = await service.StartNewGameAsync(player.CharacterId);
        var session3 = await service.StartNewGameAsync(player.CharacterId);

        await service.AddScoreAsync(session1.SessionId, 100);
        await service.AddScoreAsync(session2.SessionId, 300);
        await service.AddScoreAsync(session3.SessionId, 200);

        await service.CompleteGameVictoryAsync(session1.SessionId);
        await service.CompleteGameVictoryAsync(session2.SessionId);
        await service.CompleteGameVictoryAsync(session3.SessionId);

        var topScores = await service.GetTopScoresAsync(10);

        Assert.True(topScores[0].Score >= topScores[1].Score);
        Assert.True(topScores[1].Score >= topScores[2].Score);
    }

    /// <summary>
    /// Vérifie que DeleteSavedGameAsync retourne false si session inexistante.
    /// </summary>
    [Fact]
    public async Task DeleteSavedGameAsync_ShouldReturnFalse_WhenSessionNotExists()
    {
        var (_, service) = CreateService();

        var deleted = await service.DeleteSavedGameAsync(999);

        Assert.False(deleted);
    }

    /// <summary>
    /// Vérifie que ResumeGameAsync retourne null si session inexistante.
    /// </summary>
    [Fact]
    public async Task ResumeGameAsync_ShouldReturnNull_WhenSessionNotExists()
    {
        var (_, service) = CreateService();

        var resumed = await service.ResumeGameAsync(999);

        Assert.Null(resumed);
    }

    /// <summary>
    /// Vérifie que SaveGameAsync retourne null si la session n'est pas InProgress.
    /// </summary>
    [Fact]
    public async Task SaveGameAsync_ShouldReturnNull_WhenSessionNotInProgress()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);
        await service.CompleteGameVictoryAsync(session.SessionId);

        var saved = await service.SaveGameAsync(session.SessionId, 5);

        Assert.Null(saved);
    }

    /// <summary>
    /// Vérifie que la date de création est définie.
    /// </summary>
    [Fact]
    public async Task StartNewGameAsync_ShouldSetCreatedAt()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();

        var session = await service.StartNewGameAsync(player.CharacterId);

        Assert.True(session.CreatedAt != default);
    }

    /// <summary>
    /// Vérifie que la date de mise à jour est définie.
    /// </summary>
    [Fact]
    public async Task StartNewGameAsync_ShouldSetUpdatedAt()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();

        var session = await service.StartNewGameAsync(player.CharacterId);

        Assert.True(session.UpdatedAt != default);
    }

    /// <summary>
    /// Vérifie que le CurrentRoomId est défini à la première salle.
    /// </summary>
    [Fact]
    public async Task StartNewGameAsync_ShouldSetCurrentRoomId()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();

        var session = await service.StartNewGameAsync(player.CharacterId, DungeonLevel.Easy, 5);

        Assert.True(session.CurrentRoomId > 0);
    }

    /// <summary>
    /// Vérifie que plusieurs pénalités de fuite s'accumulent.
    /// </summary>
    [Fact]
    public async Task ApplyRunAwayPenaltyAsync_ShouldAccumulate()
    {
        var (context, service) = CreateService();
        var player = context.Player.First();
        var session = await service.StartNewGameAsync(player.CharacterId);

        await service.ApplyRunAwayPenaltyAsync(session.SessionId);
        await service.ApplyRunAwayPenaltyAsync(session.SessionId);

        var updated = await service.GetSessionByIdAsync(session.SessionId);
        Assert.NotNull(updated);
        Assert.Equal(-100, updated.Score);
    }
}
