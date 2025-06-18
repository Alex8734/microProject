using Mx.Persistence.Model;
using System.Linq;

namespace Mx.Responses;

public class TrackResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public double LengthInKm { get; set; }
    public TrackDifficulty Difficulty { get; set; }
    public string? ImageUrl { get; set; }
    public List<CoordinateResponse> Coordinates { get; set; } = new List<CoordinateResponse>();

    public static TrackResponse FromTrack(Track t) => new TrackResponse
    {
        Id = t.Id,
        Name = t.Name,
        LengthInKm = t.LengthInKm,
        Difficulty = t.Difficulty,
        ImageUrl = t.ImageUrl,
        Coordinates = t.Coordinates
            .OrderBy(c => c.SequenceNumber)
            .Select(c => new CoordinateResponse
            {
                Latitude = c.Latitude,
                Longitude = c.Longitude
            })
            .ToList()
    };
}

public class CoordinateResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
