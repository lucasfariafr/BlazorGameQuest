namespace SharedModels.Models.Environment;

/// <summary>
/// Représente une salle dans le donjon.
/// </summary>
public class Room
{
    /// <summary>
    /// Identifiant unique de la salle.
    /// </summary>
    [Required]
    public required int RoomId { get; set; }

    /// <summary>
    /// Description de la salle.
    /// </summary>
    [Required]
    public required string Description { get; set; } = string.Empty;

    /// <summary>
    /// Actions disponibles dans cette salle.
    /// </summary>
    [Required]
    public required List<AvailableActions> Actions { get; set; } = new();

    /// <summary>
    /// Monstre présent dans la salle (peut être null).
    /// </summary>
    public Monster? Monster { get; set; }
    
    /// <summary>
    /// Coffre présent dans la salle (peut être null).
    /// </summary>
    public Chest? Chest { get; set; }
}
