namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// Résultat complet d'un combat.
/// </summary>
public record FightResultDto
{
    public required List<FightTurnDto> Turns { get; init; }
    public required PlayerSnapshotDto PlayerSnapshot { get; init; }
    public required MonsterSnapshotDto MonsterSnapshot { get; init; }
    public required string Result { get; init; }
    public required int TotalTurns { get; init; }
}