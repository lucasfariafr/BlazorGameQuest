using System.Text.Json.Serialization;
using BlazorGame.SharedModels.Constants;
using BlazorGame.SharedModels.Models.Utils;

namespace BlazorGame.SharedModels.Models.Entities;

/// <summary>
/// Représente le joueur dans le jeu.
/// </summary>
public class Player : Character
{

    /// <summary>
    /// Identifiant de l'utilisateur Keycloak associé à ce joueur.
    /// Un utilisateur ne peut avoir qu'un seul joueur.
    /// </summary>
    [JsonPropertyOrder(0)]
    public string? UserId { get; set; }

    /// <summary>
    /// Nombre de coeurs restants du joueur.
    /// </summary>
    [JsonPropertyOrder(1)]
    public int HeartNumber { get; set; } = GameConstants.MaxHearts;

    /// <summary>
    /// Liste des potions en possession du joueur.
    /// </summary>
    [JsonPropertyOrder(7)]
    public virtual List<Potion>? Potions { get; set; }

    /// <summary>
    /// Indique si le joueur est actif (non banni/désactivé).
    /// </summary>
    [JsonPropertyOrder(8)]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date de création du compte joueur.
    /// </summary>
    [JsonPropertyOrder(9)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
