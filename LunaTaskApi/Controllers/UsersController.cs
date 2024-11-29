using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LunaTaskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordService _passwordService;

        public UsersController(AppDbContext context, JwtService jwtService, PasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        // Регістрація нового користувача
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            //перевірємо чи є вже користувач
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email || u.Username == userDto.Username))
                return BadRequest("Username or Email already exists.");

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = _passwordService.HashPassword(userDto.Password) // зашифровуємо пароль
            };
            // додаємо до БД
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Авторизація користувача
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // отримуємо дані користувача
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == loginDto.Email || u.Username == loginDto.Username);

            if (user == null || !_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash)) // розшифровуємо пароль
                return Unauthorized();

            var token = _jwtService.GenerateToken(user);// генеруємо токен
            return Ok(new { Token = token });
        }
    }

}