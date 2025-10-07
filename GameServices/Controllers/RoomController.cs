using GameServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Models;

namespace GameServices.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Room>> GetAllRooms()
    {
        var rooms = _roomService.GetAllRooms();
        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Room> GetRoomById(int id)
    {
        var room = _roomService.GetRoomById(id);

        if (room is null)
        {
            return NotFound(new { message = $"La salle avec l'ID {id} n'existe pas." });
        }

        return Ok(room);
    }
}
