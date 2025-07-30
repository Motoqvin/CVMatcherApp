using System.Threading.Tasks;
using CVMatcherApp.Api.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace CVMatcherApp.Api.Controllers;

[ApiController]
[Route("api/")]
public class CVController : ControllerBase
{
    private readonly ICVService _cvService;
    public CVController(ICVService cvService)
    {
        _cvService = cvService;
    }


    [HttpGet("/cv")]
    public async Task<IActionResult> GetCVs()
    {
        var cvs = await _cvService.GetAllCVsAsync();
        return Ok(cvs);
    }

    [HttpPost("/cv/upload")]
    [Consumes("multipart/form-data")]
    public IActionResult UploadCV(IFormFile formFile)
    {
        _cvService.SaveCVAsync(formFile);
        return CreatedAtAction(nameof(GetCVs), new { Message = "CV created successfully" });
    }

    [HttpGet("/cv/analyze")]
    public async Task<IActionResult> AnalyzeCV(int cvId)
    {
        await _cvService.AnalyzeCVAsync(cvId);
        return Ok(new { Message = "CV analysis started successfully" });
    }

    [HttpGet("/analytics/usage-stats")]
    public IActionResult GetUsageStats()
    {
        // This method would typically return usage statistics.
        // For now, we return a placeholder response.
        return Ok(new { Message = "Usage statistics are not implemented yet." });
    }

    [HttpGet("/analytics/user-stats")]
    public IActionResult GetUserStats()
    {
        // This method would typically return user statistics.
        // For now, we return a placeholder response.
        return Ok(new { Message = "User statistics are not implemented yet." });
    }

    [HttpGet("/health")]
    public IActionResult HealthCheck()
    {
        // This method is a simple health check endpoint.
        return Ok(new { Status = "Healthy" });
    }
}