using SharedModels.Enums;

namespace SharedModels.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public MonsterType? Monster { get; set; }
        public List<PlayerAction> AvailableActions { get; set; } = new List<PlayerAction>();
    }
}