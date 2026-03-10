using Microsoft.AspNetCore.Mvc;
using TrainingManagement.API.Attributes;
using TrainingManagement.Application.DTOs.UserTraining;
using TrainingManagement.Application.Interfaces;
using TrainingManagement.Application.Exceptions;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserTrainingsController : ControllerBase
{
    private readonly IUserTrainingService _userTrainingService;
    private readonly ILogger<UserTrainingsController> _logger;

    public UserTrainingsController(
        IUserTrainingService userTrainingService, 
        ILogger<UserTrainingsController> logger)
    {
        _userTrainingService = userTrainingService;
        _logger = logger;
    }

    [HttpGet("my-trainings")]
    public async Task<ActionResult<IEnumerable<UserTrainingDto>>> GetMyTrainings()
    {
        var userId = HttpContext.GetUserId();
        var trainings = await _userTrainingService.GetUserTrainingsAsync(userId);
        return Ok(trainings);
    }

    [HttpGet("user/{userId}")]
    [Authorize(UserRole.Admin)]
    public async Task<ActionResult<IEnumerable<UserTrainingDto>>> GetUserTrainings(int userId)
    {
        var trainings = await _userTrainingService.GetUserTrainingsAsync(userId);
        return Ok(trainings);
    }

    [HttpGet("all")]
    [Authorize(UserRole.Admin)]
    public async Task<ActionResult<IEnumerable<UserTrainingDto>>> GetAllUserTrainings()
    {
        var trainings = await _userTrainingService.GetAllUserTrainingsAsync();
        return Ok(trainings);
    }

    [HttpPut("status")]
    public async Task<IActionResult> UpdateTrainingStatus(UpdateTrainingStatusDto updateDto)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var userTrainings = await _userTrainingService.GetUserTrainingsAsync(userId);
            
            if (!userTrainings.Any(ut => ut.Id == updateDto.UserTrainingId))
            {
                return Forbid();
            }

            await _userTrainingService.UpdateTrainingStatusAsync(updateDto);
            _logger.LogInformation("Training status updated for UserTraining {UserTrainingId}", updateDto.UserTrainingId);
            return Ok(new { message = "Training status updated successfully" });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("assign")]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> AssignTrainingToUser(AssignTrainingDto assignDto)
    {
        try
        {
            await _userTrainingService.AssignTrainingToUserAsync(assignDto);
            _logger.LogInformation("Training {TrainingId} assigned to User {UserId}", assignDto.TrainingId, assignDto.UserId);
            return Ok(new { message = "Training assigned successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("remove/{userId}/{trainingId}")]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> RemoveTrainingFromUser(int userId, int trainingId)
    {
        try
        {
            await _userTrainingService.RemoveTrainingFromUserAsync(userId, trainingId);
            _logger.LogInformation("Training {TrainingId} removed from User {UserId}", trainingId, userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}