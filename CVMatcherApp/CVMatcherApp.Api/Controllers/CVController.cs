using System.Security.Claims;
using System.Threading.Tasks;
using CVMatcherApp.Api.Jobs;
using CVMatcherApp.Api.Responses;
using CVMatcherApp.Api.Services.Base;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVMatcherApp.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Roles = "User")]
public class CVController : ControllerBase
{
    private readonly ICVService _cvService;
    public CVController(ICVService cvService)
    {
        _cvService = cvService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCVs()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier);
        var cvs = await _cvService.GetAllCVsAsync(userId!.ToString().Split(": ")[1]);
        return Ok(cvs);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCVAsync(IFormFile formFile)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier);
        await _cvService.SaveCVAsync(formFile, userId!.ToString().Split(": ")[1]);
        return CreatedAtAction(nameof(GetCVs), new { Message = "CV created successfully" });
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeCV(int cvId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier);
        var cv = await _cvService.GetCVById(cvId);
        System.Console.WriteLine(cv.Content);
        System.Console.WriteLine(userId);
        BackgroundJob.Enqueue<OpenAIAnalysisJob>(job => job.AnalyzeAndSaveCVAsync(userId!.ToString(), cv));
        
        return Ok(new { Message = "CV analysis started successfully" });
    }

    
}