namespace GameServices.Interfaces;

/// <summary>
/// Interface pour initialiser les salles (<see cref="Room"/>).
/// </summary>
public interface IRoomInitializer
{
    /// <summary>
    /// Initialise et retourne la liste complète des salles du jeu.
    /// </summary>
    /// <returns>
    /// Une liste en lecture seule (<see cref="IReadOnlyList{Room}"/>) contenant toutes les salles initialisées.
    /// </returns>
    IReadOnlyList<Room> InitializeRooms();
}
