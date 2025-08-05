using System.Threading.Tasks;
using CVMatcherApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVMatcherApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _analyticsService;

    public AnalyticsController(AnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }


    [HttpGet("usage-stats")]
    public async Task<IActionResult> GetUsageStats()
    {
        var stats = await _analyticsService.GetUsageStatsAsync();
        return Ok(stats);
    }

    [HttpGet("user-stats")]
    public async Task<IActionResult> GetUserStatsAsync()
    {
        var stats = await _analyticsService.GetUserStatsAsync();
        return Ok(stats);
    }

    [HttpGet("/health")]
    public IActionResult HealthCheck()
    {
        return Ok(new {Status = "Healthy"});
    }
}