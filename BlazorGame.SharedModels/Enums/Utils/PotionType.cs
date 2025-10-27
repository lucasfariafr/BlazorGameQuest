namespace BlazorGame.SharedModels.Enums.Utils;

/// <summary>
/// Types de potions disponibles dans le jeu.
/// </summary>
public enum PotionType
{
    
    /// <summary>
    /// Potion de vie - restaure les points de vie.
    /// </summary>
    [Display(Name = "Potion de vie")]
    Health = 1,

    /// <summary>
    /// Potion de force - augmente les dégâts.
    /// </summary>
    [Display(Name = "Potion de dégat")]
    Strength = 2,

}
