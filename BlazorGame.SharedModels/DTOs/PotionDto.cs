namespace BlazorGame.SharedModels.DTOs;

/// <summary>
/// DTO pour représenter une potion.
/// </summary>
public record PotionDto
{
    /// <summary>
    /// Identifiant de la potion.
    /// </summary>
    public required int PotionId { get; init; }

    /// <summary>
    /// Type de la potion (Health, Strength).
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Effet de la potion.
    /// </summary>
    public required double Effect { get; init; }
}
