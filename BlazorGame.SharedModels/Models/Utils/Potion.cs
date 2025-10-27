namespace BlazorGame.SharedModels.Models.Utils;

/// <summary>
/// Représente une potion dans le jeu.
/// </summary>
public class Potion
{

    /// <summary>
    /// Identifiant unique de la potion.
    /// </summary>
    [Required]
    public required int PotionId { get; set; }

    /// <summary>
    /// Type de potion.
    /// </summary>
    [Required]
    public required PotionType Type { get; set; }
 
    /// <summary>
    /// Effet de la potion basé sur son type.
    /// </summary>
    public double Effect
    {
        get
        {
            if (PotionStats.PotionEffects.TryGetValue(Type, out var effect))
                return effect;

            throw new InvalidOperationException($"L'effet n'a pas été défini pour la potion : {Type}");
        }
    }

    /// <summary>
    /// Description de l'effet de la potion.
    /// </summary>
    public string Description => Type switch
    {
        PotionType.Health => $"Restaure {Effect} points de vie.",
        PotionType.Strength => $"Augmente la force de {Effect}.",
        _ => throw new InvalidOperationException($"La description n'est pas définie pour la potion : {Type}")
    };    
    
}
