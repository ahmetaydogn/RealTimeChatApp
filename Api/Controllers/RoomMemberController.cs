using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomMemberController : ControllerBase
    {
        private readonly IRoomMemberService roomMemberService;

        public RoomMemberController(IRoomMemberService _roomMemberService)
        {
            roomMemberService = _roomMemberService;
        }

        [HttpGet("GetRoomMembers")]
        public async Task<IReadOnlyCollection<RoomMember>> GetRoomMembers(Guid roomId)
        {
            return await roomMemberService.GetRoomMembersAsync(roomId);
        }
    }
}
