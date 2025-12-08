namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// Résultat d'une action effectuée par le joueur.
/// </summary>
public class ActionResultDto
{
    /// <summary>
    /// Indique si l'action a réussi.
    /// </summary>
    public required bool Success { get; set; }

    /// <summary>
    /// Message décrivant le résultat de l'action.
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Type d'action effectuée.
    /// </summary>
    public required string ActionType { get; set; }

    /// <summary>
    /// Identifiant de la salle suivante (null si game over ou fin du donjon).
    /// </summary>
    public int? NextRoomId { get; set; }

    /// <summary>
    /// Indique si la partie est terminée (mort du joueur).
    /// </summary>
    public bool IsGameOver { get; set; }

    /// <summary>
    /// Indique si le donjon est terminé (toutes les salles parcourues).
    /// </summary>
    public bool IsDungeonCompleted { get; set; }

    /// <summary>
    /// Récompense obtenue (arme, potion, etc.).
    /// </summary>
    public object? Reward { get; set; }

    /// <summary>
    /// Dégâts subis par le joueur (si applicable).
    /// </summary>
    public double? DamageTaken { get; set; }

    /// <summary>
    /// État actuel du joueur après l'action.
    /// </summary>
    public PlayerSnapshotDto? PlayerState { get; set; }

    /// <summary>
    /// Score actuel du joueur.
    /// </summary>
    public int? Score { get; set; }
}
