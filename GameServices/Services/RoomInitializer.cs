using GameServices.Interfaces;
using SharedModels.Enums;
using SharedModels.Models;

namespace GameServices.Services;

public class RoomInitializer : IRoomInitializer
{
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
            },
        }.AsReadOnly();
    }
}
