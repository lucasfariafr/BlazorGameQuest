namespace BlazorGame.SharedModels.Models;

/// <summary>
/// Représente une session de jeu (partie sauvegardée).
/// </summary>
public class GameSession
{
    /// <summary>
    /// Identifiant unique de la session.
    /// </summary>
    [Required]
    [Key]
    public int SessionId { get; set; }

    /// <summary>
    /// Identifiant du joueur associé à cette session.
    /// </summary>
    [Required]
    public required int PlayerId { get; set; }

    /// <summary>
    /// Joueur associé à cette session.
    /// </summary>
    public virtual Player? Player { get; set; }

    /// <summary>
    /// Identifiant du donjon en cours.
    /// </summary>
    [Required]
    public required int DungeonId { get; set; }

    /// <summary>
    /// Donjon en cours.
    /// </summary>
    public virtual Dungeon? Dungeon { get; set; }

    /// <summary>
    /// Identifiant de la salle actuelle.
    /// </summary>
    public int CurrentRoomId { get; set; }

    /// <summary>
    /// Score actuel du joueur.
    /// </summary>
    public int Score { get; set; } = 0;

    /// <summary>
    /// Statut de la session.
    /// </summary>
    public GameSessionStatus Status { get; set; } = GameSessionStatus.InProgress;

    /// <summary>
    /// Date de création de la session.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de dernière mise à jour.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Statut d'une session de jeu.
/// </summary>
public enum GameSessionStatus
{
    InProgress,
    Saved,
    Victory,
    Defeat,
    Abandoned
}
