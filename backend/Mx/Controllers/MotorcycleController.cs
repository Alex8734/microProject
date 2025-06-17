using Microsoft.AspNetCore.Mvc;
using Mx.Core.Services;
using Mx.Persistence.Model;

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
    public async Task<ActionResult<Motorcycle>> CreateMotorcycle(string model, string number, int horsepower, int? trackId)
    {
        var result = await _motorcycleService.AddMotorcycleAsync(model, number, horsepower, trackId);
        return result.Match<ActionResult<Motorcycle>>(
            motorcycle => Ok(motorcycle),
            error => BadRequest(error.Message)
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Motorcycle>> GetMotorcycle(int id)
    {
        var result = await _motorcycleService.GetMotorcycleByIdAsync(id, false);
        return result.Match<ActionResult<Motorcycle>>(
            motorcycle => Ok(motorcycle),
            _ => NotFound()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<Motorcycle>>> GetAllMotorcycles()
    {
        var motorcycles = await _motorcycleService.GetAllMotorcyclesAsync();
        return Ok(motorcycles);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyCollection<Motorcycle>>> GetAvailableMotorcycles()
    {
        var motorcycles = await _motorcycleService.GetAvailableMotorcyclesAsync();
        return Ok(motorcycles);
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
    public async Task<ActionResult> RentMotorcycle(int id, [FromQuery] int userId)
    {
        var result = await _motorcycleService.RentMotorcycleAsync(id, userId);
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