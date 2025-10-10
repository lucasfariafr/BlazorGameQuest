using SharedModels.Enums;

namespace SharedModels.Models;

public class Room
{
    public required int Id { get; set; }
    public required string Description { get; set; }
    public MonsterType? Monster { get; set; }
    public required IReadOnlyList<PlayerAction> AvailableActions { get; set; } 
}
