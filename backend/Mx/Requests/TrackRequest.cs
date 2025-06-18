using Mx.Persistence.Model;

namespace Mx.Requests;

public class TrackRequest
{
    public string Name { get; set; } = default!;
    public double LengthInKm { get; set; }
    public TrackDifficulty Difficulty { get; set; }
}
