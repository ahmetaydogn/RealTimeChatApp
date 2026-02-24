using Core.Entities;
using Entities.Enums;

namespace Entities.Concrete
{
    public class AppUser : BaseEntity
    {
        public string Username { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; }
        public string PasswordHash { get; set; } = null!;

        public UserType UserType { get; set; } = UserType.Normal;

        public ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
