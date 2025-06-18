using Mx.Persistence.Model;

namespace Mx.Dtos;

public class TrackDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public double LengthInKm { get; set; }
    public TrackDifficulty Difficulty { get; set; }

    public static TrackDto FromTrack(Track t) => new TrackDto
    {
        Id = t.Id,
        Name = t.Name,
        LengthInKm = t.LengthInKm,
        Difficulty = t.Difficulty
    };
}
