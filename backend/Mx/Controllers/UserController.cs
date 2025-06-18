using Microsoft.AspNetCore.Mvc;
using Mx.Core.Services;
using Mx.Persistence.Model;
using Mx.Requests;
using Mx.Responses;

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
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] UserRequest request)
    {
        var result = await _userService.AddUserAsync(request.Ssn, request.Name, request.Age, request.Weight);
        return result.Match<ActionResult<UserResponse>>(
            user => Ok(UserResponse.FromUser(user)),
            error => BadRequest(error.Message)
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(string ssn)
    {
        var result = await _userService.GetUserBySsnAsync(ssn, false);
        return result.Match<ActionResult<UserResponse>>(
            user => Ok(UserResponse.FromUser(user)),
            _ => NotFound()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<UserResponse>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users.Select(UserResponse.FromUser).ToList());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string ssn)
    {
        var result = await _userService.DeleteUserAsync(ssn);
        return result.Match<ActionResult>(
            _ => NoContent(),
            _ => NotFound()
        );
    }
}
