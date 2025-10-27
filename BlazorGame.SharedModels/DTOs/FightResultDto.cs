namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// Représente le résultat d'un combat.
/// </summary>
public record FightResultDto
{

    /// <summary>
    ///  Liste des tours réalisés.
    /// </summary>
    public required List<FightTurnDto> Turns { get; init; }

    /// <summary>
    /// État du joueur après le combat.
    /// </summary>
    public required PlayerSnapshotDto PlayerSnapshot { get; init; }

    /// <summary>
    /// État du monstre après le combat.
    /// </summary>
    public required MonsterSnapshotDto MonsterSnapshot { get; init; }

    /// <summary>
    /// Résultat du combat.
    /// </summary>
    public required string Result { get; init; }

    /// <summary>
    /// Nombre de tour total.
    /// </summary>
    public required int TotalTurns { get; init; }

}
