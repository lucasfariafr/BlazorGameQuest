namespace BlazorGame.SharedModels.Models.Environment;

/// <summary>
/// Représente un coffre dans le jeu.
/// </summary>
public class Chest
{ 

    /// <summary>
    /// Identifiant unique du coffre.
    /// </summary>
    [Required]
    public required int ChestId { get; set; }

    /// <summary>
    /// Indique si le coffre a été ouvert.
    /// </summary>
    public bool IsOpened { get; set; }

    /// <summary>
    /// Arme contenue dans le coffre (peut être null).
    /// </summary>
    public virtual Weapon? Weapon { get; set; }

    /// <summary>
    /// Potion contenue dans le coffre (peut être null).
    /// </summary>
    public virtual Potion? Potion { get; set; }
    
}
