using Microsoft.EntityFrameworkCore;
using Mx.Persistence.Model;
using Mx.Persistence.Util;
using Mx.TestInt.Util;

namespace Mx.TestInt;

public class TrackTestBase(WebApiTestFixture webApiFixture) : WebApiTestBase(webApiFixture) {
    protected override async ValueTask ImportSeedDataAsync(DatabaseContext context) {
        // Erstelle einige Teststrecken
        var easyTrack = new Track
        {
            Name = "Easy Test Track",
            LengthInKm = 3.5,
            Difficulty = TrackDifficulty.Easy
        };
        
        var mediumTrack = new Track
        {
            Name = "Medium Test Track",
            LengthInKm = 5.2,
            Difficulty = TrackDifficulty.Medium
        };
        
        var hardTrack = new Track
        {
            Name = "Hard Test Track",
            LengthInKm = 7.8,
            Difficulty = TrackDifficulty.Hard
        };
        
        var expertTrack = new Track
        {
            Name = "Expert Test Track",
            LengthInKm = 10.5,
            Difficulty = TrackDifficulty.Expert
        };
        
        context.Tracks.Add(easyTrack);
        context.Tracks.Add(mediumTrack);
        context.Tracks.Add(hardTrack);
        context.Tracks.Add(expertTrack);
        
        await context.SaveChangesAsync();
    }
    
    protected async Task<Track> GetTrackByNameAsync(string name)
    {
        Track? track = null;
        await ModifyDatabaseContentAsync(async context =>
        {
            track = await context.Tracks.FirstOrDefaultAsync(t => t.Name == name);
        });
        return track ?? throw new InvalidOperationException($"Track with name '{name}' not found");
    }
}
