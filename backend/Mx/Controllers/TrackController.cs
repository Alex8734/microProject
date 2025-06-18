using Microsoft.AspNetCore.Mvc;
using Mx.Core.Services;
using Mx.Persistence.Model;
using Mx.Dtos;

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
    public async Task<ActionResult<TrackDto>> CreateTrack(string name, double lengthInKm, TrackDifficulty difficulty)
    {
        var result = await _trackService.AddTrackAsync(name, lengthInKm, difficulty);
        return result.Match<ActionResult<TrackDto>>(
            track => Ok(TrackDto.FromTrack(track)),
            error => BadRequest(error.Message)
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrackDto>> GetTrack(int id)
    {
        var result = await _trackService.GetTrackByIdAsync(id, false);
        return result.Match<ActionResult<TrackDto>>(
            track => Ok(TrackDto.FromTrack(track)),
            _ => NotFound()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TrackDto>>> GetAllTracks()
    {
        var tracks = await _trackService.GetAllTracksAsync();
        return Ok(tracks.Select(TrackDto.FromTrack).ToList());
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
