using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixteenSounds.Data;
using SixteenSounds.Models;

namespace SixteenSounds.Controllers
{
    [Route("api/[controller]")]                                                                       //definicja adresu URL. [controller] automatycznie zamieni się na "Samples".
    [ApiController]                                                                                   //Ta linijka włącza automatyczne funkcje API (np. walidację czy dane są poprawne)
    public class SampleController : ControllerBase                                                    //dziedziczymy po ControllerBase, co daje nam dostęp do mettod typu Ok(), BadRequest() czy CreatedAtAction()
    {
        private readonly SixteenSoundsDbContext _context;                                             //Prywatne pole które pozwala nam komunikować się z bazą danych.

        public SamplesController(SixteenSoundsDbContext context)                                      //Konstruktor - serwer wstrzykuje nam dostęp do bazy.
        {
            _context = context;                                                                       //Przypisujemy bazę do naszego pola, żeby móc jej używać w innych metodach
        }

                                                                                                      // GET: api/Samples - pobiera wszystkie lub filtruje po kategorii
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sample>>> GetSamples(string? category = null)      //public - oznacza że metoda jest widoczna "na zewnątrz" - async to pojęcie związane z bazami danych,
                                                                                                      //z perspektywy procesora zgarnięcie informacji z bazy danych zajmuję bardzo długo - dlatego async działa asynchronicznie z bazą danych
                                                                                                      //- z tym pojęciem łączy się też ticket czyli nasze ostre nawiasy, w środku nich znajduję się obietnica np. to taki bilet w kolejce -
                                                                                                      //w przyszłości dostaniesz tutaj wynik określonego typu, ActionResult to jest opakowanie dla odpowiedzi HTTP,
                                                                                                      //w API potrzebne nam są nie tylko dane ale też statusy czyli np. wrzucam sampel w formacie .wav API odpowiada mi "wszystko spoko",
                                                                                                      //wrzucam zły format "wiem o co ci chodzi ale to jest niezgodne z moimi zasadami" oraz np. 404 czyli "szukałem tego dla ciebie ale tego nie ma".
                                                                                                      //IEnumerable<Sample> to najbardziej podstawowy interfejs dla kolekcji (listy, tablice) np. mówię - wynikiem będzie jakiś zbiór obiektów
                                                                                                      //typu Sample + jest taki że na ten moment nie muszę decydować czy to będzie sztywna lista czy coś innego - na przyszłość to dobra praktyka
                                                                                                      //w programowaniu do interfejsu.
                                                                                                      //GetSamples to po prostu nazwa mojej metody. W API nazwa metody często nie ma znaczenia dla adresu URL (URL definiuje [Route])
                                                                                                      //(string? category = null) string? - znak zapytania oznacza Nullable. Mówi: "Kategoria może byuć tekstem, ale może też być pusta (null)",
                                                                                                      //= null to wartość domyślna. Jeśli użytkownik wejdzie na api/Samples (bez podawania kategoriii), system podstawi null.
        {                                                                                             // Pozwala to na użycie jednej metody do dwóch celów - pobierania wszystkiego (jeśli brak kategorii) Lub filtrowania
                                                                                                      // (jeśli  kategoria jest podana, np. api/Samples? category=Drums).
            var query = _context.Samples.AsQueryable();                                               // PODSUMOWUJĄC - ta linijka mówi serwerowi: "Cześć, stwórz publiczną, nowoczesną i asynchroniczną funkcję, która obiecuje listę sampli muzycznych (lub błąd HTTP). Pozwól użytkownikowi opcjonalnie przefiltrować te sample po nazwie kategorii."

            if (!string.IsNullOrEmpty(category)) { 
            
            
            query = query.Where(s => s.Category == category);
            
            
            }
            return await query.ToListAsync();


    }

}
