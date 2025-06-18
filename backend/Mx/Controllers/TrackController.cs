using Microsoft.AspNetCore.Mvc;
using Mx.Core.Services;
using Mx.Persistence.Model;
using Mx.Requests;
using Mx.Responses;

namespace Mx.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackController : ControllerBase
{
    private readonly ITrackService _trackService;

    public TrackController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    [HttpPost]
    public async Task<ActionResult<TrackResponse>> CreateTrack([FromBody] TrackRequest request)
    {
        var result = await _trackService.AddTrackAsync(request.Name, request.LengthInKm, request.Difficulty);
        return result.Match<ActionResult<TrackResponse>>(
            track => Ok(TrackResponse.FromTrack(track)),
            error => BadRequest(error.Message)
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrackResponse>> GetTrack(int id)
    {
        var result = await _trackService.GetTrackByIdAsync(id, false);
        return result.Match<ActionResult<TrackResponse>>(
            track => Ok(TrackResponse.FromTrack(track)),
            _ => NotFound()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TrackResponse>>> GetAllTracks()
    {
        var tracks = await _trackService.GetAllTracksAsync();
        return Ok(tracks.Select(TrackResponse.FromTrack).ToList());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTrack(int id)
    {
        var result = await _trackService.DeleteTrackAsync(id);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => BadRequest(error.Message)
        );
    }
}
