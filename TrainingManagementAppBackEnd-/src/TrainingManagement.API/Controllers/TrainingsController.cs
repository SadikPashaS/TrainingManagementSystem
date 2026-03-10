using Microsoft.AspNetCore.Mvc;
using TrainingManagement.API.Attributes;
using TrainingManagement.Application.DTOs.Training;
using TrainingManagement.Application.Interfaces;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TrainingsController : ControllerBase
{
    private readonly ITrainingService _trainingService;
    private readonly ILogger<TrainingsController> _logger;

    public TrainingsController(ITrainingService trainingService, ILogger<TrainingsController> logger)
    {
        _trainingService = trainingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrainingDto>>> GetAllTrainings()
    {
        var trainings = await _trainingService.GetAllTrainingsAsync();
        return Ok(trainings);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<TrainingDto>>> GetActiveTrainings()
    {
        var trainings = await _trainingService.GetActiveTrainingsAsync();
        return Ok(trainings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrainingDto>> GetTrainingById(int id)
    {
        var training = await _trainingService.GetTrainingByIdAsync(id);
        if (training == null)
        {
            return NotFound();
        }
        return Ok(training);
    }

    [HttpPost]
    [Authorize(UserRole.Admin)]
    public async Task<ActionResult<TrainingDto>> CreateTraining(CreateTrainingDto createTrainingDto)
    {
        var training = await _trainingService.CreateTrainingAsync(createTrainingDto);
        _logger.LogInformation("Training {TrainingName} created", training.TrainingName);
        return CreatedAtAction(nameof(GetTrainingById), new { id = training.Id }, training);
    }

    [HttpPut("{id}")]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> UpdateTraining(int id, UpdateTrainingDto updateTrainingDto)
    {
        try
        {
            await _trainingService.UpdateTrainingAsync(id, updateTrainingDto);
            _logger.LogInformation("Training {TrainingId} updated", id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
[Authorize(UserRole.Admin)]
public async Task<IActionResult> DeleteTraining(int id)
{
    try
    {
        await _trainingService.DeleteTrainingAsync(id);
        return NoContent(); // This returns 204 No Content
    }
    catch (KeyNotFoundException)
    {
        return NotFound();
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
}