using Microsoft.AspNetCore.Mvc;
using Mx.Core.Services;
using Mx.Persistence.Model;
using Mx.Requests;
using Mx.Responses;

namespace Mx.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MotorcycleController : ControllerBase
{
    private readonly IMotorcycleService _motorcycleService;

    public MotorcycleController(IMotorcycleService motorcycleService)
    {
        _motorcycleService = motorcycleService;
    }

    [HttpPost]
    public async Task<ActionResult<MotorcycleResponse>> CreateMotorcycle([FromBody] MotorcycleRequest request)
    {
        var result = await _motorcycleService.AddMotorcycleAsync(request.Model, request.Number, request.Horsepower, request.TrackId);
        return result.Match<ActionResult<MotorcycleResponse>>(
            motorcycle => Ok(MotorcycleResponse.FromMotorcycle(motorcycle)),
            error => BadRequest(error.Message)
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MotorcycleResponse>> GetMotorcycle(int id)
    {
        var result = await _motorcycleService.GetMotorcycleByIdAsync(id, false);
        return result.Match<ActionResult<MotorcycleResponse>>(
            motorcycle => Ok(MotorcycleResponse.FromMotorcycle(motorcycle)),
            _ => NotFound()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<MotorcycleResponse>>> GetAllMotorcycles()
    {
        var motorcycles = await _motorcycleService.GetAllMotorcyclesAsync();
        return Ok(motorcycles.Select(MotorcycleResponse.FromMotorcycle).ToList());
    }

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyCollection<MotorcycleResponse>>> GetAvailableMotorcycles()
    {
        var motorcycles = await _motorcycleService.GetAvailableMotorcyclesAsync();
        return Ok(motorcycles.Select(MotorcycleResponse.FromMotorcycle).ToList());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMotorcycle(int id)
    {
        var result = await _motorcycleService.DeleteMotorcycleAsync(id);
        return result.Match<ActionResult>(
            _ => NoContent(),
            _ => NotFound()
        );
    }

    [HttpPost("{id}/rent")]
    public async Task<ActionResult> RentMotorcycle(int id, [FromQuery] string userSsn)
    {
        var result = await _motorcycleService.RentMotorcycleAsync(id, userSsn);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => BadRequest(error.Message)
        );
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult> ReturnMotorcycle(int id)
    {
        var result = await _motorcycleService.ReturnMotorcycleAsync(id);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => BadRequest(error.Message)
        );
    }
}
