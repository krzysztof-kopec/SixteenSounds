using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixteenSounds.Data;
using SixteenSounds.Models;

namespace SixteenSounds.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SixteenSoundsDbContext _context;

        // Konstruktor - wstrzykujemy tu nasz łącznik z bazą (Dependency Injection)
        public UsersController(SixteenSoundsDbContext context)
        {
            _context = context;
        }

        // GET: api/Users - pobiera wszystkich użytkowników
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // POST: api/Users - dodaje nowego użytkownika
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
    }
}