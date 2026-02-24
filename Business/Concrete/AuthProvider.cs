using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Concrete
{
    public class AuthProvider : IAuthService
    {
        private readonly IAppUserDal userDal;
        private readonly IConfiguration config;

        public AuthProvider(IAppUserDal _userDal, IConfiguration _config)
        {
            userDal = _userDal;
            config = _config;
        }

        public async Task<string> Register(RegisterDto registerDto)
        {
            var existing = await userDal.GetAsync(x => x.Username == registerDto.Username);

            if (existing != null)
                throw new Exception("Kullanıcı zaten mevcut.");

            var user = new AppUser()
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            await userDal.AddAsync(user);
            return GenerateToken(user);
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var existing = await userDal.GetAsync(x => x.Username == loginDto.Username);

            if (existing == null || BCrypt.Net.BCrypt.Verify(loginDto.Password, existing.PasswordHash))
            {
                throw new Exception("Kullanıcı adı veya şifre hatalı");
            }

            return GenerateToken(existing);
        }

        private string GenerateToken(AppUser user)
        {
            var secretKey = config["Jwt:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(config["ExpirationTimeInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
