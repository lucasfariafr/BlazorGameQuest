namespace SharedModels.Models.Entities;

/// <summary>
/// Représente le joueur dans le jeu.
/// </summary>
public class Player : Character
{
    /// <summary>
    /// Nombre de cœurs restants du joueur.
    /// </summary>
    [JsonPropertyOrder(1)]
    public int HeartNumber { get; set; } = GameConstants.MaxHearts;

    /// <summary>
    /// Liste des potions en possession du joueur.
    /// </summary>
    [JsonPropertyOrder(7)]
    public List<Potion>? Potions { get; set; } = new();
}
