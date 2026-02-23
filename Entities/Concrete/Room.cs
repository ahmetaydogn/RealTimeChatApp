using Core.Entities;

namespace Entities.Concrete
{
    public class Room : BaseEntity
    {
        // If it's true then normal users cannot write a message to that room
        public bool AdminOnly { get; set; } = false;

        public ICollection<RoomMember> Members { get; set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
