namespace BlazorGame.SharedModels.Models.Entities;

/// <summary>
/// Représente un monstre dans le jeu.
/// </summary>
public class Monster : Character
{

    /// <summary>
    /// Type de monstre.
    /// </summary>
    [Required]
    [JsonPropertyOrder(1)]
    public MonsterType Type { get; set; }
    
}
