using System.ComponentModel.DataAnnotations;

namespace SixteenSounds.Models
{
    public class User
    {
        [Key] // To mówi bazie danych, że to jest unikalny ID (Primary Key)
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // Nigdy nie trzymamy haseł otwartym tekstem!

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}