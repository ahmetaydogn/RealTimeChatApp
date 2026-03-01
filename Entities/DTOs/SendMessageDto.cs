using Core.Entities;

namespace Entities.DTOs
{
    public class SendMessageDto : IEntityDto
    {
        public Guid RoomId { get; set; }
        public string Content { get; set; }
    }
}
