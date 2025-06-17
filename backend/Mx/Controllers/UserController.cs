using Microsoft.AspNetCore.Mvc;
using Mx.Core.Services;
using Mx.Persistence.Model;

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
    public async Task<ActionResult<User>> CreateUser(string name, int age, double weight)
    {
        var result = await _userService.AddUserAsync(name, age, weight);
        return result.Match<ActionResult<User>>(
            user => Ok(user),
            error => BadRequest(error.Message)
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var result = await _userService.GetUserByIdAsync(id, false);
        return result.Match<ActionResult<User>>(
            user => Ok(user),
            _ => NotFound()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
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