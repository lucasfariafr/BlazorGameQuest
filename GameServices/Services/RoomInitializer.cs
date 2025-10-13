using GameServices.Interfaces;
using SharedModels.Enums;
using SharedModels.Models;

namespace GameServices.Services;

/// <summary>
/// Service responsable de l'initialisation des salles (<see cref="Room"/>) pour le jeu.
/// Fournit une liste prédéfinie de salles avec leurs monstres et actions disponibles.
/// </summary>
public class RoomInitializer : IRoomInitializer
{
    /// <summary>
    /// Initialise et retourne la liste complète des salles du jeu.
    /// Chaque salle contient un identifiant unique, une description, un monstre éventuel
    /// et les actions disponibles pour le joueur.
    /// </summary>
    /// <returns>
    /// Une liste en lecture seule (<see cref="IReadOnlyList{Room}"/>) contenant toutes les salles.
    /// </returns>
    public IReadOnlyList<Room> InitializeRooms()
    {
        return new List<Room>
            {
                new()
                {
                    Id = 1,
                    Description = $"Un {MonsterType.Goblin} apparaît. Que faites-vous ?",
                    Monster = MonsterType.Goblin,
                    AvailableActions =
                    [
                        PlayerAction.Fight,
                        PlayerAction.RunAway,
                        PlayerAction.Search
                    ]
                },
                new()
                {
                    Id = 2,
                    Description = "Un coffre mystérieux !",
                    Monster = null,
                    AvailableActions =
                    [
                        PlayerAction.Open,
                        PlayerAction.Ignore
                    ]
                }
            }.AsReadOnly();
    }
}
