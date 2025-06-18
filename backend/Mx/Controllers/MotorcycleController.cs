using Microsoft.AspNetCore.Mvc;
using Mx.Core.Services;
using Mx.Persistence.Model;
using Mx.Dtos;

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
    public async Task<ActionResult<MotorcycleDto>> CreateMotorcycle(string model, string number, int horsepower, int? trackId)
    {
        var result = await _motorcycleService.AddMotorcycleAsync(model, number, horsepower, trackId);
        return result.Match<ActionResult<MotorcycleDto>>(
            motorcycle => Ok(MotorcycleDto.FromMotorcycle(motorcycle)),
            error => BadRequest(error.Message)
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MotorcycleDto>> GetMotorcycle(int id)
    {
        var result = await _motorcycleService.GetMotorcycleByIdAsync(id, false);
        return result.Match<ActionResult<MotorcycleDto>>(
            motorcycle => Ok(MotorcycleDto.FromMotorcycle(motorcycle)),
            _ => NotFound()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<MotorcycleDto>>> GetAllMotorcycles()
    {
        var motorcycles = await _motorcycleService.GetAllMotorcyclesAsync();
        return Ok(motorcycles.Select(MotorcycleDto.FromMotorcycle).ToList());
    }

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyCollection<MotorcycleDto>>> GetAvailableMotorcycles()
    {
        var motorcycles = await _motorcycleService.GetAvailableMotorcyclesAsync();
        return Ok(motorcycles.Select(MotorcycleDto.FromMotorcycle).ToList());
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
