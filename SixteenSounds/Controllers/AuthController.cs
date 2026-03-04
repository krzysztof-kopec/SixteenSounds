using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixteenSounds.Data;
using SixteenSounds.Models;
using SixteenSounds.Controllers;

namespace SixteenSounds.Controllers
{
    [Route("api/[controller]")]
    [ApiController] //sprawdzamy bezpieczeństwo i poprawność danych
    public class AuthController : ControllerBase
    {
        private readonly SixteenSoundsDbContext _context;

        public AuthController(SixteenSoundsDbContext context)
        {
            _context = context;
        }

        //rejestracja
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            //sprawdzamy czy użytkownik o podanym emailu już istnieje
            if(await _context.Users.AnySync(u => u.Email == request.Email))
            {
                return BadRequest("Użytkownik o podanym emailu już istnieje.");
            }

            //szyfrowanie hasla
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await Ok("Konto stworzone pomyślnie.");

        }

        //Logowanie
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        }

    }
}

