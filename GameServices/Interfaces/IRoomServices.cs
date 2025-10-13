using SharedModels.Models;

namespace GameServices.Interfaces;

/// <summary>
/// Interface pour la gestion des salles (<see cref="Room"/>).
/// </summary>
public interface IRoomService
{
    /// <summary>
    /// Récupère toutes les salles disponibles.
    /// </summary>
    /// <returns>
    /// Une liste en lecture seule (<see cref="IReadOnlyList{Room}"/>) contenant toutes les salles.
    /// </returns>
    IReadOnlyList<Room> GetAllRooms();

    /// <summary>
    /// Récupère une salle spécifique par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la salle à récupérer.</param>
    /// <returns>
    /// La <see cref="Room"/> correspondante si elle existe, sinon <see langword="null"/>.
    /// </returns>
    Room? GetRoomById(int id);
}
