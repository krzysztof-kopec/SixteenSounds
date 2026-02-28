namespace SixteenSounds.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Relacja powrotna do sampli
        public ICollection<Sample> Samples { get; set; } = new List<Sample>();
    }
}