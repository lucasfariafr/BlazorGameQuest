namespace BlazorGame.SharedModels.Models.Utils;

/// <summary>
/// Représente une arme dans le jeu.
/// </summary>
public class Weapon
{

    /// <summary>
    /// Identifiant unique de l'arme.
    /// </summary>
    [Required]
    public required int WeaponId { get; set; }

    /// <summary>
    /// Type d'arme.
    /// </summary>
    [Required]
    public required WeaponType Type { get; set; }

    /// <summary>
    /// Dégâts de l'arme basés sur son type.
    /// </summary>
    public double Damage
    {
        get
        {
            if (WeaponStats.WeaponDamages.TryGetValue(Type, out var damage))
                return damage;

            throw new InvalidOperationException($"Les dégâts n'ont pas été définis pour l'arme : {Type}");
        }
    }
}
