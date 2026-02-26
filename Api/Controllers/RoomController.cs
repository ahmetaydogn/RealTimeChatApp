using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomDal roomDal;

        public RoomController(IRoomDal _roomDal)
        {
            roomDal = _roomDal;
        }

        [HttpPost("PostRoom")]
        public async Task<IActionResult> AddRoom(RoomDto room)
        {
            // worst validation
            if (room.RoomName.IsNullOrEmpty())
            {
                return BadRequest();
            }

            var roomToAdd = new Room()
            {
                RoomName = room.RoomName,
                AdminOnly = room.AdminOnly
            };

            await roomDal.AddAsync(roomToAdd);
            return Ok();
            
        }

        [HttpGet("GetRooms")]
        public async Task<IReadOnlyCollection<Room>> GetRooms()
        {
            var content = await roomDal.GetAllAsync();
            return content;
        }
    }
}
