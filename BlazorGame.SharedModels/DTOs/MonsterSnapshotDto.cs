namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// Snapshot du monstre après le combat.
/// </summary>
public record MonsterSnapshotDto
{
    public required int CharacterId { get; init; }
    public required string Type { get; init; }
    public required double Health { get; init; }
    public required double Strength { get; init; }
    public required double Armor { get; init; }
    public required double Damage { get; init; }
    public string? WeaponType { get; init; }
}