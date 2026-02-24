using Core.Entities;

namespace Entities.DTOs
{
    public class RegisterDto : IEntityDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
