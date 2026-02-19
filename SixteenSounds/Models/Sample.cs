using System.ComponentModel.DataAnnotations;

namespace SixteenSounds.Models
{
    public class Sample
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty; // Ścieżka do pliku na dysku/serwerze

        public string Category { get; set; } = string.Empty; // Np. "Drums", "Synth", "Vocal"

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Relacja: jeden sampel należy do jednego użytkownika
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}