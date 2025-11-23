namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// Arguments d'événement pour signaler la fin d'une action.
/// </summary>
public class ActionCompletedEventArgs
{
    /// <summary>
    /// Indique si le jeu est terminé (joueur mort).
    /// </summary>
    public bool IsGameOver { get; set; }

    /// <summary>
    /// Indique si le donjon est terminé.
    /// </summary>
    public bool IsDungeonCompleted { get; set; }

    /// <summary>
    /// Identifiant de la prochaine salle (null si fin du donjon).
    /// </summary>
    public int? NextRoomId { get; set; }

    /// <summary>
    /// Message à afficher au joueur.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Score actuel du joueur.
    /// </summary>
    public int? Score { get; set; }
}
