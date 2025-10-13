using System.ComponentModel.DataAnnotations;

namespace SharedModels.Enums;

/// <summary>
/// Représente les différentes actions qu’un joueur peut effectuer dans une salle.
/// Chaque valeur correspond à une action spécifique disponible dans le jeu.
/// </summary>
public enum PlayerAction
{
    /// <summary>
    /// Le joueur choisit de combattre un ennemi présent dans la salle.
    /// </summary>
    [Display(Name = "Combattre")]
    Fight = 0,

    /// <summary>
    /// Le joueur choisit de fuir la salle pour éviter le combat.
    /// </summary>
    [Display(Name = "Fuir")]
    RunAway = 1,

    /// <summary>
    /// Le joueur choisit de chercher des objets ou indices dans la salle.
    /// </summary>
    [Display(Name = "Chercher")]
    Search = 2,

    /// <summary>
    /// Le joueur choisit d’ouvrir un élément (coffre, piège, etc.).
    /// </summary>
    [Display(Name = "Ouvrir")]
    Open = 3,

    /// <summary>
    /// Le joueur choisit d’ignorer les éléments de la salle et de ne rien faire.
    /// </summary>
    [Display(Name = "Ignorer")]
    Ignore = 4
}
