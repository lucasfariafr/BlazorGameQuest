using System.ComponentModel.DataAnnotations;

namespace SharedModels.Enums;

/// <summary>
/// Définit les différents types de monstres pouvant apparaître dans une salle.
/// Chaque type de monstre possède ses propres caractéristiques.
/// </summary>
public enum MonsterType
{
    /// <summary>
    /// Le gobelin est une créature faible mais rusée, souvent rencontrée en grand nombre.
    /// </summary>
    [Display(Name = "Goblin")]
    Goblin = 0,

    /// <summary>
    /// L'ogre est un monstre puissant, redouté pour sa force physique.
    /// </summary>
    [Display(Name = "Ogre")]
    Ogre = 1,

    /// <summary>
    /// Le zombie est un mort-vivant lent mais persistant, souvent difficile à éliminer définitivement.
    /// </summary>
    [Display(Name = "Zombie")]
    Zombie = 2
}
