namespace Mx.Persistence.Model;

public class TrackCoordinate
{
    public int Id { get; set; }
    public int TrackId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int SequenceNumber { get; set; }
    
    // Navigation property
    public Track Track { get; set; } = null!;
}
