using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService _authService)
        {
            authService = _authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var token = await authService.Register(dto);
            if (token == null)
            {
                return BadRequest("Kullanıcı oluşturulamadı.");
            }

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await authService.Login(dto);
            if (token == null)
            {
                return BadRequest("Geçersiz kullanıcı adı veya şifre.");
            }

            return Ok(new { token });
        }
    }
}
