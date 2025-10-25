namespace SharedModels.Models.Entities;

/// <summary>
/// Représente un personnage de base dans le jeu.
/// </summary>
public abstract class Character
{
    /// <summary>
    /// Identifiant unique du personnage.
    /// </summary>
    [Required]
    [JsonPropertyOrder(0)]
    public required int CharacterId { get; set; }

    /// <summary>
    /// Points de vie actuels du personnage.
    /// </summary>
    [JsonPropertyOrder(2)]
    public double Health { get; set; } = GameConstants.MaxHealth;

    /// <summary>
    /// Force du personnage.
    /// </summary>
    [Required]
    [JsonPropertyOrder(3)]
    public required double Strength { get; set; }

    /// <summary>
    /// Armure du personnage.
    /// </summary>
    [Required]
    [JsonPropertyOrder(4)]
    public required double Armor { get; set; }

    /// <summary>
    /// Arme équipée par le personnage.
    /// </summary>
    [JsonPropertyOrder(5)]
    public Weapon? Weapon { get; set; }

    /// <summary>
    /// Dégâts totaux du personnage (force + dégâts de l'arme).
    /// </summary>
    [JsonPropertyOrder(6)]
    public double Damage => (Weapon?.Damage ?? 0) + Strength;
}
