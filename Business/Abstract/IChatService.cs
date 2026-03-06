using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IChatService
    {
        Task SendMessageAsync(Guid senderId, SendMessageDto dto);
        Task<IReadOnlyCollection<Message>> GetRoomHistoryAsync(Guid roomId);
    }
}
