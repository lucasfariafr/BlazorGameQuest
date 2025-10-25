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
        var zombie = new Monster
        {
            CharacterId = 1,
            Type = MonsterType.Zombie,
            Strength = 12,
            Armor = 6,
            Weapon = new Weapon
            {
                WeaponId = 1,
                Type = WeaponType.Sword
            }
        };

        var chest = new Chest
        {
            ChestId = 1,
            Potion = new Potion
            {
                PotionId = 1,
                Type = PotionType.Health
            }
        };
        
        return new List<Room>
        {
                new()
                {
                    RoomId = 1,
                    Description = $"Un {zombie.Type.ToString().ToLower()} apparaît. Que faites-vous ?",
                    Monster = zombie,
                    Actions =
                    [
                        AvailableActions.Fight,
                        AvailableActions.RunAway,
                        AvailableActions.Search
                    ]
                },
                new()
                {
                    RoomId = 2,
                    Description = "Un coffre mystérieux !",
                    Monster = null,
                    Actions =
                    [
                        AvailableActions.Open,
                        AvailableActions.Ignore
                    ],
                    Chest = chest
                }
            }.AsReadOnly();
    }
}
