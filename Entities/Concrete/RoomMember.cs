using Core.Entities;
using Entities.Enums;

namespace Entities.Concrete
{

    public class RoomMember : IEntity
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public RoomRole Role { get; set; } = RoomRole.Member;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
