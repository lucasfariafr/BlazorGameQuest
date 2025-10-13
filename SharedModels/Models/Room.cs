using SharedModels.Enums;

namespace SharedModels.Models;

/// <summary>
/// Représente une salle dans le jeu.
/// Chaque salle possède un identifiant unique, une description,
/// éventuellement un monstre, ainsi qu'une liste d'actions disponibles pour le joueur.
/// </summary>
public class Room
{
    /// <summary>
    /// Identifiant unique de la salle.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Description textuelle de la salle.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Type de monstre présent dans la salle, s'il y en a un.
    /// La valeur peut être <see langword="null"/> s'il n'y a pas de monstre.
    /// </summary>
    public MonsterType? Monster { get; set; }

    /// <summary>
    /// Liste des actions que le joueur peut effectuer dans cette salle.
    /// </summary>
    public required IReadOnlyList<PlayerAction> AvailableActions { get; set; }
}
