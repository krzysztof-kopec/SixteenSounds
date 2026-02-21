using System.ComponentModel.DataAnnotations.Schema; // Ważne: dodaj to na samej górze!

namespace SixteenSounds.Models
{
    public class Sample
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // TEGO CI BRAKOWAŁO:
        [NotMapped] // To mówi bazie danych: "Nie twórz dla mnie kolumny, potrzebuję tego pola tylko w kodzie"
        public string FileUrl { get; set; } = string.Empty;
    }
}