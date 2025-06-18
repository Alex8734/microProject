using Mx.Persistence;
using Mx.Persistence.Model;
using Microsoft.EntityFrameworkCore;
using Mx.Persistence.Util;

namespace Mx.Util;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await SeedDatabaseAsync(dbContext);
    }
    
    private static async Task SeedDatabaseAsync(DatabaseContext context)
    {
        // Überprüfen, ob bereits Daten vorhanden sind
        if (await context.Tracks.AnyAsync() && await context.Users.AnyAsync() && await context.Motorcycles.AnyAsync())
        {
            return; // Wenn bereits Daten vorhanden sind, nichts tun
        }
        
        // Benutzer erstellen, falls noch keiner existiert
        if (!await context.Users.AnyAsync())
        {
            var defaultUser = new User
            {
                Name = "Max Mustermann",
                Age = 35,
                Weight = 82.5,
                Ssn = "123-45-6789"
            };
            
            context.Users.Add(defaultUser);
            await context.SaveChangesAsync();
        }
        
        // Tracks erstellen, falls noch keine existieren
        if (!await context.Tracks.AnyAsync())
        {
            var tracks = new List<Track>
            {
                new Track
                {
                    Name = "Hockenheimring",
                    LengthInKm = 4.574,
                    Difficulty = TrackDifficulty.Medium,
                    ImageUrl = "https://www.hockenheimring.de/uploads/tx_templavoila/Porsche_Turn_1_01.jpg",
                    Coordinates = new List<TrackCoordinate>
                    {
                        new() { Latitude = 49.329639, Longitude = 8.569189, SequenceNumber = 1 },
                        new() { Latitude = 49.331939, Longitude = 8.566271, SequenceNumber = 2 },
                        new() { Latitude = 49.332767, Longitude = 8.565756, SequenceNumber = 3 },
                        new() { Latitude = 49.332981, Longitude = 8.566228, SequenceNumber = 4 },
                        new() { Latitude = 49.333095, Longitude = 8.567387, SequenceNumber = 5 },
                        new() { Latitude = 49.332767, Longitude = 8.569017, SequenceNumber = 6 },
                        new() { Latitude = 49.329753, Longitude = 8.574811, SequenceNumber = 7 },
                        new() { Latitude = 49.328697, Longitude = 8.575669, SequenceNumber = 8 },
                        new() { Latitude = 49.327698, Longitude = 8.575240, SequenceNumber = 9 },
                        new() { Latitude = 49.326584, Longitude = 8.572750, SequenceNumber = 10 },
                        new() { Latitude = 49.329639, Longitude = 8.569189, SequenceNumber = 11 }
                    }
                },
                new Track
                {
                    Name = "Nürburgring",
                    LengthInKm = 5.148,
                    Difficulty = TrackDifficulty.Hard,
                    ImageUrl = "https://nuerburgring.de/fileadmin/_processed_/b/d/csm_ring-2021-gp-2-c-gruppe-c_d758bd4726.jpg",
                    Coordinates = new List<TrackCoordinate>
                    {
                        new() { Latitude = 50.335556, Longitude = 6.947222, SequenceNumber = 1 },
                        new() { Latitude = 50.336111, Longitude = 6.949444, SequenceNumber = 2 },
                        new() { Latitude = 50.335833, Longitude = 6.951389, SequenceNumber = 3 },
                        new() { Latitude = 50.334722, Longitude = 6.952778, SequenceNumber = 4 },
                        new() { Latitude = 50.333889, Longitude = 6.953333, SequenceNumber = 5 },
                        new() { Latitude = 50.332500, Longitude = 6.952500, SequenceNumber = 6 },
                        new() { Latitude = 50.331944, Longitude = 6.950556, SequenceNumber = 7 },
                        new() { Latitude = 50.333056, Longitude = 6.949444, SequenceNumber = 8 },
                        new() { Latitude = 50.333889, Longitude = 6.947222, SequenceNumber = 9 },
                        new() { Latitude = 50.335556, Longitude = 6.947222, SequenceNumber = 10 }
                    }
                },
                new Track
                {
                    Name = "Sachsenring",
                    LengthInKm = 3.671,
                    Difficulty = TrackDifficulty.Medium,
                    ImageUrl = "https://www.sachsenring-circuit.com/images/uploads/slider/6169b04370a5d.jpg",
                    Coordinates = new List<TrackCoordinate>
                    {
                        new() { Latitude = 50.794167, Longitude = 12.689444, SequenceNumber = 1 },
                        new() { Latitude = 50.796111, Longitude = 12.689722, SequenceNumber = 2 },
                        new() { Latitude = 50.797500, Longitude = 12.691111, SequenceNumber = 3 },
                        new() { Latitude = 50.798056, Longitude = 12.694167, SequenceNumber = 4 },
                        new() { Latitude = 50.797222, Longitude = 12.696944, SequenceNumber = 5 },
                        new() { Latitude = 50.795556, Longitude = 12.697778, SequenceNumber = 6 },
                        new() { Latitude = 50.793611, Longitude = 12.696667, SequenceNumber = 7 },
                        new() { Latitude = 50.792778, Longitude = 12.694444, SequenceNumber = 8 },
                        new() { Latitude = 50.792778, Longitude = 12.691667, SequenceNumber = 9 },
                        new() { Latitude = 50.794167, Longitude = 12.689444, SequenceNumber = 10 }
                    }
                }
            };
            
            context.Tracks.AddRange(tracks);
            await context.SaveChangesAsync();
        }
        
        // Motorräder erstellen, falls noch keine existieren und Tracks vorhanden sind
        if (!await context.Motorcycles.AnyAsync())
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Ssn == "123-45-6789");
            var hockenheimring = await context.Tracks.FirstOrDefaultAsync(t => t.Name == "Hockenheimring");
            var nuerburgring = await context.Tracks.FirstOrDefaultAsync(t => t.Name == "Nürburgring");
            var sachsenring = await context.Tracks.FirstOrDefaultAsync(t => t.Name == "Sachsenring");
            
            if (hockenheimring != null && nuerburgring != null && sachsenring != null)
            {
                var motorcycles = new List<Motorcycle>
                {
                    new Motorcycle
                    {
                        Model = "Honda CBR 1000RR-R Fireblade",
                        Number = "CBR-001",
                        Horsepower = 217,
                        Track = hockenheimring
                    },
                    new Motorcycle
                    {
                        Model = "Kawasaki Ninja ZX-10R",
                        Number = "ZX10-002",
                        Horsepower = 203,
                        Track = nuerburgring
                    },
                    new Motorcycle
                    {
                        Model = "Ducati Panigale V4",
                        Number = "PAN-003",
                        Horsepower = 214,
                        Track = sachsenring
                    },
                    new Motorcycle
                    {
                        Model = "BMW S 1000 RR",
                        Number = "BMW-004",
                        Horsepower = 207,
                        Track = hockenheimring
                    },
                    new Motorcycle
                    {
                        Model = "Yamaha YZF-R1",
                        Number = "YZF-005",
                        Horsepower = 200,
                        Track = nuerburgring,
                        RentedBySsn = user!.Ssn
                    },
                    new Motorcycle
                    {
                        Model = "Aprilia RSV4",
                        Number = "RSV-006",
                        Horsepower = 217,
                        Track = sachsenring
                    }
                };
                
                context.Motorcycles.AddRange(motorcycles);
                await context.SaveChangesAsync();
            }
        }
    }
}
