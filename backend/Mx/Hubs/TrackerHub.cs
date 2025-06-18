using Microsoft.AspNetCore.SignalR;
using Mx.Persistence.Model;

namespace Mx.Hubs;

public interface ITrackerHub
{
    public ValueTask UpdatePositionsAsync();
}

public class TrackerHub : Hub, ITrackerHub
{
    public ValueTask UpdatePositionsAsync() => throw new NotImplementedException();
}

public class TrackTrackerListResponse
{
    public string TrackName { get; set; }
    public int TrackId { get; set; }
    public List<TrackerInfoResponse> Trackers { get; set; } = [];
}
public class TrackerInfoResponse
{
    public Motorcycle Motorcycle { get; set; }
    public string TrackerId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    
}

public class TrackerPositionResponse
{
    
}