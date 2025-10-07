using GameServices.Interfaces;
using SharedModels.Models;

namespace GameServices.Services;

public class RoomService : IRoomService
{
    private readonly IReadOnlyList<Room> _rooms;

    public RoomService(IRoomInitializer roomInitializer)
    {
        _rooms = roomInitializer.InitializeRooms();
    }

    public IReadOnlyList<Room> GetAllRooms() => _rooms;

    public Room? GetRoomById(int id) => _rooms.FirstOrDefault(r => r.Id == id);
}
