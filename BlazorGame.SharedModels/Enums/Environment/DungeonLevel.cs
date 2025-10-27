namespace BlazorGame.SharedModels.Enums.Environment;

/// <summary>
/// Niveaux disponibles pour un donjon.
/// </summary>
public enum DungeonLevel
{

    /// <summary>
    /// Niveau facile.
    /// </summary>
    [Display(Name = "Facile")]
    Easy = 1,

    /// <summary>
    /// Niveau moyen.
    /// </summary>
    [Display(Name = "Moyen")]
    Medium = 2,

    /// <summary>
    /// Niveau difficile.
    /// </summary>
    [Display(Name = "Difficile")]
    Difficult = 3,
}
