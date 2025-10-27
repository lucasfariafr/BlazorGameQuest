namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// Snapshot du joueur après le combat.
/// </summary>
public record PlayerSnapshotDto
{
    public required int CharacterId { get; init; }
    public required double Health { get; init; }
    public required double Strength { get; init; }
    public required double Armor { get; init; }
    public required double Damage { get; init; }
    public required int HeartNumber { get; init; }
    public string? WeaponType { get; init; }
}