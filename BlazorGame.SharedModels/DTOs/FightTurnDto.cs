namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// Représente un tour de combat.
/// </summary>
public record FightTurnDto
{
    public required int TurnNumber { get; init; }
    public required double PlayerHealth { get; init; }
    public required double MonsterHealth { get; init; }
    public required double PlayerDamageDealt { get; init; }
    public required double MonsterDamageDealt { get; init; }
}