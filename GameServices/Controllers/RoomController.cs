using Microsoft.AspNetCore.Mvc;
using GameServices.Interfaces;
using SharedModels.Models;
using System.Collections.Generic;

namespace GameServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomInitializer _roomInitializer;

        public RoomController(IRoomInitializer roomInitializer)
        {
            _roomInitializer = roomInitializer;
        }

        [HttpGet]
        public ActionResult<List<Room>> GetRooms()
        {
            return Ok(_roomInitializer.InitializeRooms());
        }
    }
}