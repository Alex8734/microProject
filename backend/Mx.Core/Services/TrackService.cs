using Mx.Persistence;
using Mx.Persistence.Model;
using Mx.Persistence.Util;
using OneOf;
using OneOf.Types;

namespace Mx.Core.Services;

public interface ITrackService
{
    public ValueTask<OneOf<Track, ValidationError>> AddTrackAsync(string name, double lengthInKm, TrackDifficulty difficulty);
    public ValueTask<OneOf<Track, NotFound>> GetTrackByIdAsync(int id, bool tracking);
    public ValueTask<IReadOnlyCollection<Track>> GetAllTracksAsync();
    public ValueTask<OneOf<Success, ValidationError>> DeleteTrackAsync(int id);
}

public sealed class TrackService(IUnitOfWork uow) : ITrackService
{
    private const double MinLength = 0.5; // 500m
    private const double MaxLength = 50.0; // 50km

    public async ValueTask<OneOf<Track, ValidationError>> AddTrackAsync(string name, double lengthInKm, TrackDifficulty difficulty)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ValidationError { Message = "Name cannot be empty" };
        }

        if (lengthInKm < MinLength || lengthInKm > MaxLength)
        {
            return new ValidationError { Message = $"Length must be between {MinLength} and {MaxLength} km" };
        }

        var track = uow.TrackRepository.AddTrack(name, lengthInKm, difficulty);
        await uow.SaveChangesAsync();
        return track;
    }

    public async ValueTask<OneOf<Track, NotFound>> GetTrackByIdAsync(int id, bool tracking)
    {
        var track = await uow.TrackRepository.GetTrackByIdAsync(id, tracking);
        return track is null ? new NotFound() : track;
    }

    public async ValueTask<IReadOnlyCollection<Track>> GetAllTracksAsync()
    {
        IReadOnlyCollection<Track> tracks = await uow.TrackRepository.GetAllTracksAsync(false);
        return tracks;
    }

    public async ValueTask<OneOf<Success, ValidationError>> DeleteTrackAsync(int id)
    {
        var track = await uow.TrackRepository.GetTrackByIdAsync(id, true);

        if (track is null)
        {
            return new ValidationError { Message = "Track not found" };
        }

        if (track.AvailableMotorcycles.Any())
        {
            return new ValidationError { Message = "Cannot delete track with assigned motorcycles" };
        }

        uow.TrackRepository.RemoveTrack(track);
        await uow.SaveChangesAsync();
        return new Success();
    }
} 