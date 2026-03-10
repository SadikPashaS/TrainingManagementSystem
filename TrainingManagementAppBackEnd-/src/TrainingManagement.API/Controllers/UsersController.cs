using Microsoft.AspNetCore.Mvc;
using TrainingManagement.API.Attributes;
using TrainingManagement.Application.DTOs.User;
using TrainingManagement.Application.Interfaces;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(UserRole.Admin)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        // Users can only view their own profile unless they are admin
        var currentUserId = HttpContext.GetUserId();
        var currentUserRole = HttpContext.GetUserRole();

        if (currentUserRole != UserRole.Admin && currentUserId != id)
        {
            return Forbid();
        }

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [Authorize(UserRole.Admin)]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(createUserDto);
            _logger.LogInformation("User {UserId} created by admin", user.UserId);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
    {
        try
        {
            await _userService.UpdateUserAsync(id, updateUserDto);
            _logger.LogInformation("User {UserId} updated", id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            _logger.LogInformation("User {UserId} deleted", id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}