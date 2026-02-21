using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixteenSounds.Data;
using SixteenSounds.Models;
using Microsoft.OpenApi.Models;

namespace SixteenSounds.Controllers
{
    [Route("api/[controller]")]                                                                       //definicja adresu URL. [controller] automatycznie zamieni się na "Samples".
    [ApiController]                                                                                   //Ta linijka włącza automatyczne funkcje API (np. walidację czy dane są poprawne)
    public class SampleController : ControllerBase                                                    //dziedziczymy po ControllerBase, co daje nam dostęp do mettod typu Ok(), BadRequest() czy CreatedAtAction()
    {
        private readonly SixteenSoundsDbContext _context;                                             //Prywatne pole które pozwala nam komunikować się z bazą danych.

        public SampleController(SixteenSoundsDbContext context)                                      //Konstruktor - serwer wstrzykuje nam dostęp do bazy.
        {
            _context = context;                                                                       //Przypisujemy bazę do naszego pola, żeby móc jej używać w innych metodach
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sample>>> GetSamples(string? category = null)
        {
            var query = _context.Samples.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(s => s.Category == category);
            }

            var samples = await query.ToListAsync();

            // Tworzymy bazowy adres URL (np. http://localhost:5159)
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // Dla każdego sampla "doklejamy" link do pliku
            foreach (var sample in samples)
            {
                sample.FileUrl = $"{baseUrl}/samples/{sample.FileName}";
            }

            return Ok(samples);
        }


        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // Dodaj tę linię! Mówi Swaggerowi, że to formularz z plikiem
        public async Task<IActionResult> UploadSample(
        IFormFile file,           // Usuń [FromForm] sprzed każdego z osobna
        [FromForm] string name,
        [FromForm] string category,
        [FromForm] int userId)
        {
            // 1. Walidacja formatu
            var allowedExtensions = new[] { ".mp3", ".wav", ".ogg" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Niedozwolony format pliku. Akceptujemy tylko .mp3, .wav, .ogg");

            // 2. Walidacja rozmiaru (np. 15MB)
            long maxFileSize = 15 * 1024 * 1024; // 15MB w bajtach
            if (file.Length > maxFileSize)
                return BadRequest("Plik jest za duży. Maksymalny rozmiar to 15MB.");

            // 3. Czyszczenie nazwy pliku ze spacji i dziwnych znaków
            // Zamieniamy spacje na '-' i bierzemy tylko bezpieczne znaki
            string safeFileName = Path.GetFileNameWithoutExtension(file.FileName)
                .Replace(" ", "-")
                .Replace("@", "");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "samples");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            // Generujemy unikalną, ale czystszą nazwę
            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var sample = new Sample
            {
                Name = name,
                Category = category,
                FileName = uniqueFileName,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            _context.Samples.Add(sample);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sampel dodany pomyślnie!", fileName = uniqueFileName });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSample(int id)
        {
            var sample = await _context.Samples.FindAsync(id);
            if (sample == null) return NotFound("Nie znaleziono takiego sampla w bazie.");

            // 1. Ścieżka do pliku na dysku
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "samples", sample.FileName);

            // 2. Fizyczne usuwanie pliku z folderu
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // 3. Usuwanie rekordu z bazy danych
            _context.Samples.Remove(sample);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Sampel o ID {id} został usunięty z bazy i z dysku." });
        }




    }

}
