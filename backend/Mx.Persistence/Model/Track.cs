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
    public string? ImageUrl { get; set; }  // URL zum gespeicherten Streckenbild
    
    // Koordinaten als Sammlung von TrackCoordinate-Objekten
    public ICollection<TrackCoordinate> Coordinates { get; set; } = new List<TrackCoordinate>();
    
    public ICollection<Motorcycle> AvailableMotorcycles { get; set; } = new List<Motorcycle>();
}
