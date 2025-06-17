namespace Mx.Persistence.Model;

public enum TrackDifficulty
{
    Easy = 1,
    Medium = 2,
    Hard = 3,
    Expert = 4
}

public class Track
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double LengthInKm { get; set; }  // Länge in Kilometern
    public TrackDifficulty Difficulty { get; set; }
    public ICollection<Motorcycle> AvailableMotorcycles { get; set; } = new List<Motorcycle>();
} 