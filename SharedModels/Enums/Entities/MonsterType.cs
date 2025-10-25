namespace SharedModels.Enums.Entities;

/// <summary>
/// Types de monstres disponibles dans le jeu.
/// </summary>
public enum MonsterType
{
    /// <summary>
    /// Goblin - petit monstre agile.
    /// </summary>
    [Display(Name = "Goblin")]
    Goblin = 1,

    /// <summary>
    /// Ogre - gros monstre puissant.
    /// </summary>
    [Display(Name = "Ogre")]
    Ogre = 2,

    /// <summary>
    /// Zombie - monstre mort-vivant.
    /// </summary>
    [Display(Name = "Zombie")]
    Zombie = 3
}
