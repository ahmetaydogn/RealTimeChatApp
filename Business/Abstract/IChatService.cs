using Entities.DTOs;

namespace Business.Abstract
{
    public interface IChatService
    {
        Task SendMessageAsync(Guid senderId, SendMessageDto dto);
        Task<IReadOnlyCollection<MessageCacheDto>> GetRoomHistoryAsync(Guid roomId);
        Task<bool> CanSendMessage(Guid roomId, Guid senderId);
    }
}
