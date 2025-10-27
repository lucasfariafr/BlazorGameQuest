namespace BlazorGame.SharedModels.Data;

/// <summary>
/// Statistiques des potions.
/// </summary>
public static class PotionStats
{

    /// <summary>
    /// Dictionnaire des effets par type de potion.
    /// </summary>
    public static readonly IReadOnlyDictionary<PotionType, double> PotionEffects =
        new Dictionary<PotionType, double>
        {
            [PotionType.Health] = GameConstants.RecoveryPotion,
            [PotionType.Strength] = GameConstants.StrengthPotion
        };
}
