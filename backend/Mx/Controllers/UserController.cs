using Microsoft.AspNetCore.Mvc;
using Mx.Core.Services;
using Mx.Persistence.Model;
using Mx.Dtos;

namespace Mx.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(string name, int age, double weight)
    {
        var result = await _userService.AddUserAsync(name, age, weight);
        return result.Match<ActionResult<UserDto>>(
            user => Ok(UserDto.FromUser(user)),
            error => BadRequest(error.Message)
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var result = await _userService.GetUserByIdAsync(id, false);
        return result.Match<ActionResult<UserDto>>(
            user => Ok(UserDto.FromUser(user)),
            _ => NotFound()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users.Select(UserDto.FromUser).ToList());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        return result.Match<ActionResult>(
            _ => NoContent(),
            _ => NotFound()
        );
    }
}
