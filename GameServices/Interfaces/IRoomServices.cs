using SharedModels.Models;

namespace GameServices.Interfaces;

public interface IRoomService
{
    IReadOnlyList<Room> GetAllRooms();

    Room? GetRoomById(int id);
}
