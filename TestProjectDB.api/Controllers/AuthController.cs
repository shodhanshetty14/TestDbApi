using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestProjectDB.api.Data;
using TestProjectDB.api.Models;

namespace TestProjectDB.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext dbcontext, IConfiguration config)
        {
            _dbcontext = dbcontext;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto RegistrationData)
        {
            if (await _dbcontext.Users.AnyAsync(e => e.Email == RegistrationData.Email))
            {
                return BadRequest("User already exists");
            }
            if (RegistrationData.Password != RegistrationData.ConfirmPassword)
            {
                return BadRequest("Password does not match");
            }

            var HashPass = BCrypt.Net.BCrypt.HashPassword(RegistrationData.Password);

            User userEntity = new User
            {
                Name = RegistrationData.Name,
                Email = RegistrationData.Email,
                Password = HashPass
            };

            await _dbcontext.AddAsync(userEntity);
            await _dbcontext.SaveChangesAsync();

            return Created("User created successfully", userEntity);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto LoginData)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(e => e.Email == LoginData.Email);

            if(user == null)
            {
                return NotFound("User does not exists");
            }
            if (!BCrypt.Net.BCrypt.Verify(LoginData.password, user.Password))
            {
                return BadRequest("Please check your password");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: signIn
                );
            string tokenVal = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    Token = tokenVal,
                    User = user,
                });
        }
    }
}
