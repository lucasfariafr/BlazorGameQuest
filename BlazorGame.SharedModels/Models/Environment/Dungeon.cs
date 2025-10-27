namespace BlazorGame.SharedModels.Models.Environment;

/// <summary>
/// Représente un donjon dans le jeu.
/// </summary>
public class Dungeon
{

    /// <summary>
    /// Identifiant unique du donjon.
    /// </summary>
    [Required]
    public required int DungeonId { get; set; }

    /// <summary>
    /// Niveau de difficulté du donjon.
    /// </summary>
    [Required]
    public required DungeonLevel DifficultyLevel { get; set; }

    /// <summary>
    /// Liste des salles du donjon.
    /// </summary>
    [Required]
    public virtual required List<Room> Rooms { get; set; } = new();

    /// <summary>
    /// Indique si le donjon a été complété.
    /// </summary>
    public bool IsCompleted { get; set; } = false;   
}
