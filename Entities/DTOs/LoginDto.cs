using Core.Entities;

namespace Entities.DTOs
{
    public class LoginDto : IEntityDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
