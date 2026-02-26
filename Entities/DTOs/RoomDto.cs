using Core.Entities;

namespace Entities.DTOs
{
    public class RoomDto : IEntityDto
    {
        public string RoomName { get; set; }
        public bool AdminOnly { get; set; } = false;
    }
}
