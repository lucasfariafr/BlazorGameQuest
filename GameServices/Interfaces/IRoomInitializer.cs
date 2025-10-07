using SharedModels.Models;

namespace GameServices.Interfaces
{
    public interface IRoomInitializer
    {
        IReadOnlyList<Room> InitializeRooms();
    }
}
