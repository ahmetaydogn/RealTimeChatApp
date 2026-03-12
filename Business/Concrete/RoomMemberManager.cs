using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class RoomMemberManager : IRoomMemberService
    {
        private readonly IRoomMemberDal roomMemberDal;
        private readonly IRoomDal roomDal;

        public RoomMemberManager(IRoomMemberDal _roomMemberDal, IRoomDal _roomDal)
        {
            roomMemberDal = _roomMemberDal;
            roomDal = _roomDal;
        }

        public async Task<RoomMember> GetRoomMemberAsync(Guid roomId, Guid userId)
        {
            if (roomId == Guid.Empty || userId == Guid.Empty)
            {
                throw new ArgumentException("Room Id or User Id cannot be empty.");
            }

            var existing = await roomMemberDal.GetAsync(x => x.RoomId == roomId && x.UserId == userId);
            return existing;
        }

        public async Task<IReadOnlyCollection<RoomMember>> GetRoomMembersAsync(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                throw new ArgumentException("Room Id cannot be empty.");
            }

            return await roomMemberDal.GetAllAsync(x => x.RoomId == roomId);
        }

        public async Task<bool> IsUserInRoomAsync(Guid roomId, Guid userId)
        {
            /* These part is unnecessary because the caller should check if the room exists before calling this method. But I will leave it here for safety.
    // Check if the room exists
            var room = await roomDal.GetAsync(x => x.Id == roomId);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found.");
            }
            */

            var existing = await roomMemberDal.GetAsync(x => x.RoomId == roomId && x.UserId == userId);
            if (existing == null) {
                return false;
            }

            return true;
        }
    }
}
