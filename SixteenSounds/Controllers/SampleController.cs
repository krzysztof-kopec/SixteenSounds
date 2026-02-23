using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixteenSounds.Data;
using SixteenSounds.Models;
using Microsoft.OpenApi.Models;

namespace SixteenSounds.Controllers
{
    [Route("api/[controller]")] // Adres URL: api/Sample
    [ApiController] // Automatyczna walidacja modeli
    public class SampleController : ControllerBase
    {
        private readonly SixteenSoundsDbContext _context;

        public SampleController(SixteenSoundsDbContext context)
        {
            _context = context;
        }

        // POBIERANIE LISTY SAMPLI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sample>>> GetSamples(string? category = null)
        {
            var query = _context.Samples.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(s => s.Category == category);
            }

            var samples = await query.ToListAsync();

            // Dynamiczne tworzenie adresu URL (np. http://localhost:5159)
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            foreach (var sample in samples)
            {
                sample.FileUrl = $"{baseUrl}/samples/{sample.FileName}";
            }

            return Ok(samples);
        }

        // WGRYWANIE NOWEGO PLIKU
        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // Wymagane dla Swaggera przy IFormFile
        public async Task<IActionResult> UploadSample(
            IFormFile file,
            [FromForm] string name,
            [FromForm] string category,
            [FromForm] int userId)
        {
            // 1. Walidacja formatu
            var allowedExtensions = new[] { ".mp3", ".wav", ".ogg" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Niedozwolony format pliku. Akceptujemy tylko .mp3, .wav, .ogg");

            // 2. Walidacja rozmiaru (15MB)
            long maxFileSize = 15 * 1024 * 1024;
            if (file.Length > maxFileSize)
                return BadRequest("Plik jest za duży. Maksymalny rozmiar to 15MB.");

            // 3. Czyszczenie nazwy pliku (Twoja nowa logika "Safe Name")
            var rawFileName = Path.GetFileNameWithoutExtension(file.FileName);
            var safeFileName = rawFileName
                .Replace(" ", "-")  // Spacja -> Myślnik
                .Replace("@", "")   // Usuń @
                .Replace("#", "")   // Usuń #
                .Replace(".", "-"); // Kropka wewnątrz nazwy -> Myślnik

            // Ścieżka do folderu wwwroot/samples
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "samples");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            // Generujemy unikalną nazwę z GUID
            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Zapis fizycznego pliku na dysku
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Zapis informacji do bazy danych
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

        // USUWANIE SAMPLA I PLIKU
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