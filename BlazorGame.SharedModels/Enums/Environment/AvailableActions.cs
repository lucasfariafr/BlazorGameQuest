namespace BlazorGame.SharedModels.Enums.Environment;

/// <summary>
/// Actions disponibles dans les salles du donjon.
/// </summary>
public enum AvailableActions
{

    /// <summary>
    /// Combattre le monstre présent.
    /// </summary>
    [Display(Name = "Combattre")]
    Fight = 1,

    /// <summary>
    /// Fuir la salle.
    /// </summary>
    [Display(Name = "Fuir")]
    RunAway = 2,

    /// <summary>
    /// Chercher des objets dans la salle.
    /// </summary>
    [Display(Name = "Chercher")]
    Search = 3,

    /// <summary>
    /// Ouvrir le coffre.
    /// </summary>
    [Display(Name = "Ouvrir")]
    Open = 4,

    /// <summary>
    /// Ignorer et passer à la salle suivante.
    /// </summary>
    [Display(Name = "Ignorer")]
    Ignore = 5
    
}
