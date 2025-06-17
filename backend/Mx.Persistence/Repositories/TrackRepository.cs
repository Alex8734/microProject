using Mx.Persistence.Model;
using Microsoft.EntityFrameworkCore;

namespace Mx.Persistence.Repositories;

public interface ITrackRepository
{
    public Track AddTrack(string name, double lengthInKm, TrackDifficulty difficulty);
    public ValueTask<IReadOnlyCollection<Track>> GetAllTracksAsync(bool tracking);
    public ValueTask<Track?> GetTrackByIdAsync(int id, bool tracking);
    public void RemoveTrack(Track track);
}

public sealed class TrackRepository(DbSet<Track> trackSet) : ITrackRepository
{
    private IQueryable<Track> Tracks => trackSet;
    private IQueryable<Track> TracksNoTracking => Tracks.AsNoTracking();

    public Track AddTrack(string name, double lengthInKm, TrackDifficulty difficulty)
    {
        var track = new Track
        {
            Name = name,
            LengthInKm = lengthInKm,
            Difficulty = difficulty
        };

        trackSet.Add(track);

        return track;
    }

    public async ValueTask<IReadOnlyCollection<Track>> GetAllTracksAsync(bool tracking)
    {
        IQueryable<Track> source = tracking ? Tracks : TracksNoTracking;

        List<Track> tracks = await source
            .Include(t => t.AvailableMotorcycles)
            .ToListAsync();

        return tracks;
    }

    public async ValueTask<Track?> GetTrackByIdAsync(int id, bool tracking)
    {
        IQueryable<Track> source = tracking ? Tracks : TracksNoTracking;

        var track = await source
            .Include(t => t.AvailableMotorcycles)
            .FirstOrDefaultAsync(t => t.Id == id);

        return track;
    }

    public void RemoveTrack(Track track)
    {
        trackSet.Remove(track);
    }
} 