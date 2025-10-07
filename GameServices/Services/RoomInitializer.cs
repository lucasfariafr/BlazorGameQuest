using GameServices.Interfaces;
using SharedModels.Enums;
using SharedModels.Models;

namespace GameServices.Services
{
    public class RoomInitializer : IRoomInitializer
    {
        public List<Room> InitializeRooms()
        {
            return new List<Room>
            {
                new Room
                {
                    Id = 1,
                    Description = $"Un {MonsterType.Goblin} apparaît. Que faites-vous ?",
                    Monster = MonsterType.Goblin,
                    AvailableActions = new List<PlayerAction>
                    {
                        PlayerAction.Fight,
                        PlayerAction.RunAway,
                        PlayerAction.Search
                    }
            }   ,
                new Room
                {
                    Id = 2,
                    Description = "Un coffre mystérieux !",
                    AvailableActions = new List<PlayerAction>
                    {
                        PlayerAction.Open,
                        PlayerAction.Ignore
                    }
                }
            };
        }
    }
}