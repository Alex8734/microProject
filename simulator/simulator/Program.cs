using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Grpc.Net.Client;
using Mx.Protos;
using Newtonsoft.Json;

// Klasse zum Deserialisieren der Track-Koordinaten vom Backend
public class TrackCoordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

class Program
{
    // Backend-URL zum Abrufen der Track-Koordinaten
    private static readonly string BackendUrl = Environment.GetEnvironmentVariable("BACKEND_URL") ?? "http://localhost:5000";

    // gRPC-Server-URL
    private static readonly string GrpcServerUrl = Environment.GetEnvironmentVariable("GRPC_SERVER_URL") ?? "http://localhost:5001";

    // Track-ID aus Umgebungsvariable
    private static readonly int TrackId = int.Parse(Environment.GetEnvironmentVariable("TRACK_ID") ?? "1");

    // Tracker-ID (kann aus Umgebungsvariable kommen oder generiert werden)
    private static readonly string TrackerId = Environment.GetEnvironmentVariable("TRACKER_ID") ?? Guid.NewGuid().ToString();

    // User SSN (kann aus Umgebungsvariable kommen)
    private static readonly string UserSsn = Environment.GetEnvironmentVariable("USER_SSN") ?? "123-45-6789";

    // Motorcycle ID (kann aus Umgebungsvariable kommen)
    private static readonly int MotorcycleId = int.Parse(Environment.GetEnvironmentVariable("MOTORCYCLE_ID") ?? "1");

    // Simulationsgeschwindigkeit in Millisekunden (Intervall zwischen Positionsupdates)
    private static readonly int UpdateIntervalMs = int.Parse(Environment.GetEnvironmentVariable("UPDATE_INTERVAL_MS") ?? "1000");

    static async Task Main(string[] args)
    {
        Console.WriteLine($"Starting track simulator for Track ID: {TrackId}");
        Console.WriteLine($"Tracker ID: {TrackerId}");

        try
        {
            // Track-Koordinaten vom Backend abrufen
            var coordinates = await FetchTrackCoordinates(TrackId);

            if (coordinates == null || coordinates.Count == 0)
            {
                Console.WriteLine("Fehler: Keine Track-Koordinaten gefunden.");
                return;
            }

            Console.WriteLine($"Erfolgreich {coordinates.Count} Koordinaten für Track ID {TrackId} abgerufen.");

            // Starte die Simulation
            await SimulateTracker(coordinates);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    // Holt die Track-Koordinaten vom Backend
    private static async Task<List<TrackCoordinate>> FetchTrackCoordinates(int trackId)
    {
        using var httpClient = new HttpClient();
        var url = $"{BackendUrl}/api/tracks/{trackId}/coordinates";
        Console.WriteLine($"Rufe Track-Koordinaten ab von: {url}");

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<TrackCoordinate>>(content);
    }

    // Simuliert die Bewegung des Trackers auf dem Track
    private static async Task SimulateTracker(List<TrackCoordinate> coordinates)
    {
        // Erstelle gRPC-Channel und Client
        using var channel = GrpcChannel.ForAddress(GrpcServerUrl);
        var client = new TrackerService.TrackerServiceClient(channel);

        Console.WriteLine($"gRPC-Client verbunden mit: {GrpcServerUrl}");

        // Wähle zufälligen Startpunkt
        var random = new Random();
        var currentIndex = random.Next(coordinates.Count);
        var currentCoordinate = coordinates[currentIndex];

        Console.WriteLine($"Starte an zufälliger Position: Latitude {currentCoordinate.Latitude}, Longitude {currentCoordinate.Longitude}");

        // Sende die erste Position
        await SendPosition(client, currentCoordinate);

        // Simulationsschleife
        while (true)
        {
            // Berechne nächsten Index (zyklisch durch die Liste)
            var nextIndex = (currentIndex + 1) % coordinates.Count;
            var nextCoordinate = coordinates[nextIndex];

            // Berechne einen Punkt zwischen aktuellem und nächstem Punkt
            var intermediateCoordinate = new TrackCoordinate
            {
                Latitude = currentCoordinate.Latitude + (nextCoordinate.Latitude - currentCoordinate.Latitude) * 0.1,
                Longitude = currentCoordinate.Longitude + (nextCoordinate.Longitude - currentCoordinate.Longitude) * 0.1
            };

            // Aktualisiere die aktuelle Position
            currentCoordinate = intermediateCoordinate;

            // Wenn wir nahe genug am nächsten Punkt sind, gehen wir zum nächsten Punkt in der Liste
            var distance = CalculateDistance(currentCoordinate, nextCoordinate);
            if (distance < 0.0001) // Sehr kleiner Wert für "nahe genug"
            {
                currentIndex = nextIndex;
                currentCoordinate = nextCoordinate;
            }

            // Sende die aktuelle Position
            await SendPosition(client, currentCoordinate);

            // Warte vor dem nächsten Update
            await Task.Delay(UpdateIntervalMs);
        }
    }

    // Sendet eine Position an den gRPC-Server
    private static async Task SendPosition(TrackerService.TrackerServiceClient client, TrackCoordinate coordinate)
    {
        try
        {
            var request = new PositionRequest
            {
                TrackId = TrackId,
                TrackerId = TrackerId,
                Latitude = coordinate.Latitude,
                Longitude = coordinate.Longitude,
                UserSsn = UserSsn,
                MotorcycleId = MotorcycleId,
                Timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 Format
            };

            var response = await client.SendPositionAsync(request);
            Console.WriteLine($"Position gesendet: Lat {coordinate.Latitude}, Lon {coordinate.Longitude} - Antwort: {response.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Senden der Position: {ex.Message}");
        }
    }

    // Berechnet die Distanz zwischen zwei Koordinaten (vereinfachte Version)
    private static double CalculateDistance(TrackCoordinate p1, TrackCoordinate p2)
    {
        // Einfache euklidische Distanz (nicht korrekt für Geolokalisierung, aber ausreichend für unsere Simulation)
        return Math.Sqrt(Math.Pow(p2.Latitude - p1.Latitude, 2) + Math.Pow(p2.Longitude - p1.Longitude, 2));
    }
}
