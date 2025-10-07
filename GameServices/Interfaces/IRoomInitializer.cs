using SharedModels.Models;

namespace GameServices.Interfaces
{
    public interface IRoomInitializer
    {
        List<Room> InitializeRooms();
    }
}