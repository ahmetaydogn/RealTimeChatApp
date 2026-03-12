using Core.Entities;

namespace Entities.DTOs
{
    public class MessageCacheDto : IEntityDto
    {
        public Guid SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
