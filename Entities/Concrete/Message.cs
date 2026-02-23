using Core.Entities;

namespace Entities.Concrete
{
    public class Message : BaseEntity
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public Guid SenderId { get; set; }
        public AppUser Sender { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
