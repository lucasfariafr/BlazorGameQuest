namespace BlazorGame.SharedModels.Enums.Utils;

/// <summary>
/// Types d'armes disponibles dans le jeu.
/// </summary>
public enum WeaponType
{
    /// <summary>
    /// Épée - arme de mêlée puissante.
    /// </summary>
    [Display(Name = "Épée")]
    Sword = 1,

    /// <summary>
    /// Arc - arme à distance.
    /// </summary>
    [Display(Name = "Arc")]
    Bow = 2,

    /// <summary>
    /// Bâton de pouvoir - arme magique.
    /// </summary>
    [Display(Name = "Bâton de pouvoir")]
    Wand = 3
}
