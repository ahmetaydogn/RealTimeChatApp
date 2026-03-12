using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IRoomService
    {
        Task AddRoomAsync(RoomDto roomDto);
        Task<IReadOnlyCollection<Room>> GetRoomsAsync();
        Task<Room?> GetRoomByIdAsync(Guid roomId);
        Task JoinRoomAsync(Guid roomId, Guid userId);
        Task LeaveRoomAsync(Guid roomId, Guid userId);
    }
}
