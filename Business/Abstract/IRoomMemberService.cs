using Core.DataAccess;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IRoomMemberService
    {
        Task<IReadOnlyCollection<RoomMember>> GetRoomMembersAsync(Guid roomId);
        Task<RoomMember> GetRoomMemberAsync(Guid roomId, Guid userId);

        Task<bool> IsUserInRoomAsync(Guid roomId, Guid userId);
    }
}
