namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// Représente un snapshot du joueur.
/// </summary>
public record PlayerSnapshotDto
{

    /// <summary>
    /// Identifiant unique du personnage.
    /// </summary>
    public required int CharacterId { get; init; }

    /// <summary>
    /// Nombre de coeurs restants du joueur.
    /// </summary>
    public required int HeartNumber { get; init; }

    /// <summary>
    /// Points de vie actuels du personnage.
    /// </summary>
    public required double Health { get; init; }

    /// <summary>
    /// Force du personnage.
    /// </summary>
    public required double Strength { get; init; }

    /// <summary>
    /// Armure du personnage.
    /// </summary>
    public required double Armor { get; init; }

    /// <summary>
    /// Arme équipée par le personnage.
    /// </summary>
    public required double Damage { get; init; }

    /// <summary>
    /// Dégâts totaux du personnage (force + dégâts de l'arme).
    /// </summary>
    public string? WeaponType { get; init; }

}
