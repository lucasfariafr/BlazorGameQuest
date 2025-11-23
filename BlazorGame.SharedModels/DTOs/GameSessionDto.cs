namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// DTO pour représenter une session de jeu.
/// </summary>
public record GameSessionDto
{
    /// <summary>
    /// Identifiant de la session.
    /// </summary>
    public int SessionId { get; init; }

    /// <summary>
    /// Identifiant du joueur.
    /// </summary>
    public int PlayerId { get; init; }

    /// <summary>
    /// Identifiant du donjon.
    /// </summary>
    public int DungeonId { get; init; }

    /// <summary>
    /// Identifiant de la salle actuelle.
    /// </summary>
    public int CurrentRoomId { get; init; }

    /// <summary>
    /// Score actuel.
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Statut de la session.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Date de création.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// État du joueur.
    /// </summary>
    public PlayerSnapshotDto? PlayerState { get; init; }
}

/// <summary>
/// DTO pour démarrer une nouvelle partie.
/// </summary>
public record StartGameRequestDto
{
    /// <summary>
    /// Niveau de difficulté du donjon.
    /// </summary>
    public string DifficultyLevel { get; init; } = "Easy";

    /// <summary>
    /// Nombre de salles dans le donjon.
    /// </summary>
    public int RoomCount { get; init; } = 5;
}

/// <summary>
/// DTO pour l'historique des scores.
/// </summary>
public record ScoreHistoryDto
{
    /// <summary>
    /// Identifiant de la session.
    /// </summary>
    public int SessionId { get; init; }

    /// <summary>
    /// Score final.
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Statut (Victory/Defeat).
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Date de la partie.
    /// </summary>
    public DateTime PlayedAt { get; init; }
}

/// <summary>
/// DTO pour une partie sauvegardée.
/// </summary>
public record SavedGameDto
{
    /// <summary>
    /// Identifiant de la session.
    /// </summary>
    public int SessionId { get; init; }

    /// <summary>
    /// Score actuel.
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Identifiant de la salle actuelle.
    /// </summary>
    public int CurrentRoomId { get; init; }

    /// <summary>
    /// Identifiant du donjon.
    /// </summary>
    public int DungeonId { get; init; }

    /// <summary>
    /// Identifiant du joueur.
    /// </summary>
    public int PlayerId { get; init; }

    /// <summary>
    /// Date de sauvegarde.
    /// </summary>
    public DateTime SavedAt { get; init; }

    /// <summary>
    /// État du joueur.
    /// </summary>
    public PlayerSnapshotDto? PlayerState { get; init; }
}
