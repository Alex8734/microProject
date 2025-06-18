using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mx.Hubs;
using Mx.Persistence;
using Mx.Persistence.Model;
using Mx.Persistence.Util;
using Mx.Protos;
using Mx.Responses;

namespace Mx.RPCServices;

public class TrackerService : Protos.TrackerService.TrackerServiceBase
{
    private readonly IHubContext<TrackerHub, ITrackerClient> _hubContext;
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<TrackerService> _logger;

    public TrackerService(
        IHubContext<TrackerHub, ITrackerClient> hubContext,
        DatabaseContext dbContext,
        ILogger<TrackerService> logger)
    {
        _hubContext = hubContext;
        _dbContext = dbContext;
        _logger = logger;
    }

    public override async Task<PositionResponse> SendPosition(PositionRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation(
                "Received position update for tracker {TrackerId} on track {TrackId}: ({Lat}, {Lng})",
                request.TrackerId, request.TrackId, request.Latitude, request.Longitude);

            // Daten aus der Datenbank laden
            var track = await _dbContext.Tracks
                .FirstOrDefaultAsync(t => t.Id == request.TrackId);

            if (track == null)
            {
                return new PositionResponse
                {
                    Success = false,
                    Message = $"Track with ID {request.TrackId} not found"
                };
            }

            var motorcycle = await _dbContext.Motorcycles
                .FirstOrDefaultAsync(m => m.Id == request.MotorcycleId);
            if (motorcycle == null)
            {
                return new PositionResponse
                {
                    Success = false,
                    Message = $"Motorcycle with ID {request.MotorcycleId} not found"
                };
            }
            
            var user = await _dbContext.Users
                                       .FirstOrDefaultAsync( u => u.Ssn == motorcycle.RentedBySsn );
            // Erstelle Response-Objekt
            var trackerPosition = new TrackerPositionResponse
            {
                TrackerId = request.TrackerId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Timestamp = DateTime.Parse(request.Timestamp)
            };

            var trackerInfo = new TrackerInfoResponse
            {
                TrackerId = request.TrackerId,
                Motorcycle = MotorcycleResponse.FromMotorcycle(motorcycle),
                UserName = user?.Name ?? "Unbekannt",
                Position = trackerPosition
            };

            var trackResponse = new TrackTrackerListResponse
            {
                TrackId = track.Id,
                TrackName = track.Name
            };
            trackResponse.Trackers.Add(trackerInfo);

            // Sende die Position an alle verbundenen Clients
            await _hubContext.Clients.All.UpdatePositionsAsync(trackResponse);

            return new PositionResponse
            {
                Success = true,
                Message = "Position update received and broadcast to clients"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing position update");
            return new PositionResponse
            {
                Success = false,
                Message = $"Error processing position update: {ex.Message}"
            };
        }
    }
}
