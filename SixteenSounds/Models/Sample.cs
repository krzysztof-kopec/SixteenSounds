public class Sample
{
       public int Id { get; set; } // Unikalny identyfikator dla każdego sampla
    public string Name { get; set; } = string.Empty; // Nazwa sampla
    public string Category { get; set; } = string.Empty; // Kategoria, np. "Drums", "Bass", "Synth"

    public string FileName { get; set; } = string.Empty; // Nazwa pliku, np. "kick.wav"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data dodania sampla

    public int UserId { get; set; }
}