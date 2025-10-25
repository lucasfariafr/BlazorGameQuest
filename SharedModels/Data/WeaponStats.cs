namespace SharedModels.Data;

/// <summary>
/// Statistiques des armes.
/// </summary>
public static class WeaponStats
{
    /// <summary>
    /// Dictionnaire des dégâts par type d'arme.
    /// </summary>
    public static readonly IReadOnlyDictionary<WeaponType, double> WeaponDamages =
        new Dictionary<WeaponType, double>
        {
            [WeaponType.Sword] = GameConstants.SwordDamage,
            [WeaponType.Bow] = GameConstants.BowDamage,
            [WeaponType.Wand] = GameConstants.WandDamage
        };
}
