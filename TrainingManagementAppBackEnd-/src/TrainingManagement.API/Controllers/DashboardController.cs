using Microsoft.AspNetCore.Mvc;
using TrainingManagement.API.Attributes;
using TrainingManagement.Application.DTOs.Dashboard;
using TrainingManagement.Application.Interfaces;

namespace TrainingManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<UserDashboardDto>> GetDashboard()
    {
        var userId = HttpContext.GetUserId();
        var dashboard = await _dashboardService.GetUserDashboardAsync(userId);
        return Ok(dashboard);
    }
}