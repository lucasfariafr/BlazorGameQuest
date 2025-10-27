namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// Représente un tour de combat.
/// </summary>
public record FightTurnDto
{

    /// <summary>
    /// Numéro du tour.
    /// </summary>
    public required int TurnNumber { get; init; }

    /// <summary>
    /// Vie du jouer.
    /// </summary>
    public required double PlayerHealth { get; init; }

    /// <summary>
    /// Vie du monstre.
    /// </summary>
    public required double MonsterHealth { get; init; }

    /// <summary>
    /// Dégâts infligés du joueur.
    /// </summary>
    public required double PlayerDamageDealt { get; init; }

    /// <summary>
    /// Dégâts infligés du monstre.
    /// </summary>
    public required double MonsterDamageDealt { get; init; }

}
