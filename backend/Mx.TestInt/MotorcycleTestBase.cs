using Microsoft.EntityFrameworkCore;
using Mx.Persistence.Model;
using Mx.Persistence.Util;
using Mx.TestInt.Util;

namespace Mx.TestInt;

public class MotorcycleTestBase(WebApiTestFixture webApiFixture) : WebApiTestBase(webApiFixture)
{
    protected override async ValueTask ImportSeedDataAsync(DatabaseContext context)
    {
        // Erstelle Strecken
        var easyTrack = new Track
        {
            Name = "Motorcycle Easy Track",
            LengthInKm = 4.2,
            Difficulty = TrackDifficulty.Easy
        };
        
        var hardTrack = new Track
        {
            Name = "Motorcycle Hard Track",
            LengthInKm = 8.5,
            Difficulty = TrackDifficulty.Hard
        };
        
        context.Tracks.Add(easyTrack);
        context.Tracks.Add(hardTrack);
        await context.SaveChangesAsync();
        
        // Erstelle Benutzer
        var user1 = new User
        {
            Name = "Motorcycle Test User",
            Age = 35,
            Weight = 82.4,
            Ssn = "555-55-5555"
        };
        
        context.Users.Add(user1);
        await context.SaveChangesAsync();
        
        // Erstelle Motorräder
        var motorcycle1 = new Motorcycle
        {
            Model = "Honda CBR",
            Number = "CBR-001",
            Horsepower = 130,
            Track = easyTrack
        };
        
        var motorcycle2 = new Motorcycle
        {
            Model = "Kawasaki Ninja",
            Number = "NIN-002", 
            Horsepower = 160,
            Track = hardTrack
        };
        
        var motorcycle3 = new Motorcycle
        {
            Model = "Ducati Monster",
            Number = "DUC-003",
            Horsepower = 145,
            Track = hardTrack,
            RentedBySsn = user1.Ssn,
        };
        
        context.Motorcycles.Add(motorcycle1);
        context.Motorcycles.Add(motorcycle2);
        context.Motorcycles.Add(motorcycle3);
        
        await context.SaveChangesAsync();
    }
    
    protected async Task<Motorcycle> GetMotorcycleByModelAsync(string model)
    {
        Motorcycle? motorcycle = null;
        await ModifyDatabaseContentAsync(async context =>
        {
            motorcycle = await context.Motorcycles
                .Include(m => m.Track)
                .FirstOrDefaultAsync(m => m.Model == model);
        });
        return motorcycle ?? throw new InvalidOperationException($"Motorcycle with model '{model}' not found");
    }
    
    protected async Task<User> GetUserByNameAsync(string name)
    {
        User? user = null;
        await ModifyDatabaseContentAsync(async context =>
        {
            user = await context.Users.FirstOrDefaultAsync(u => u.Name == name);
        });
        return user ?? throw new InvalidOperationException($"User with name '{name}' not found");
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
