using Microsoft.AspNetCore.SignalR;
using Mx.Persistence.Model;
using Mx.Responses;

namespace Mx.Hubs;

public interface ITrackerClient
{
    public ValueTask UpdatePositionsAsync(TrackTrackerListResponse response);
}

public class TrackerHub : Hub<ITrackerClient>
{

}

public class TrackTrackerListResponse
{
    public required string TrackName { get; set; }
    public int TrackId { get; set; }
    public List<TrackerInfoResponse> Trackers { get; set; } = [];
}
public class TrackerInfoResponse
{
    public required MotorcycleResponse Motorcycle { get; set; }
    public string TrackerId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public TrackerPositionResponse Position { get; set; } = default!;
}

public class TrackerPositionResponse
{
    public string TrackerId { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}