using Business.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService roomService;

        public RoomController(IRoomService roomService)
        {
            this.roomService = roomService;
        }

        [HttpPost("PostRoom")]
        public async Task<IActionResult> AddRoom(RoomDto room)
        {
            try
            {
                await roomService.AddRoomAsync(room);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetRooms")]
        public async Task<IReadOnlyCollection<Room>> GetRooms()
        {
            return await roomService.GetRoomsAsync();
        }

        [HttpPost("join/{roomId}")]
        [Authorize]
        public async Task<IActionResult> JoinRoom(Guid roomId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                await roomService.JoinRoomAsync(roomId, userId);
                return Ok(new { message = "User joined the room." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("leave/{roomId}")]
        [Authorize]
        public async Task<IActionResult> LeaveRoom(Guid roomId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                await roomService.LeaveRoomAsync(roomId, userId);
                return Ok(new { message = "User left the room." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
