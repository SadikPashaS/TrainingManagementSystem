using Microsoft.AspNetCore.Mvc;
using TrainingManagement.API.Attributes;
using TrainingManagement.Application.DTOs.Auth;
using TrainingManagement.Application.Interfaces;

namespace TrainingManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);
            _logger.LogInformation("User {UserId} logged in successfully", loginDto.UserId);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Failed login attempt for user {UserId}", loginDto.UserId);
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = HttpContext.GetUserId();
        await _authService.LogoutAsync(userId);
        _logger.LogInformation("User {UserId} logged out", userId);
        return Ok(new { message = "Logged out successfully" });
    }
}