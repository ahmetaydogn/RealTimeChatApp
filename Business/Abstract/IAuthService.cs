using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task<string> Login(LoginDto loginDto);

        Task<string> Register(RegisterDto registerDto);
    }
}
