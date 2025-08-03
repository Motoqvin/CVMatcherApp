using System.Security.Claims;
using System.Threading.Tasks;
using CVMatcherApp.Api.Jobs;
using CVMatcherApp.Api.Responses;
using CVMatcherApp.Api.Services;
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
    private readonly CVParserService parserService;
    public CVController(ICVService cvService, CVParserService parserService)
    {
        _cvService = cvService;
        this.parserService = parserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCVs()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cvs = await _cvService.GetAllCVsAsync(userId!);
        return Ok(cvs);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCVAsync(IFormFile formFile)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var parsedCV = await parserService.ParseCVAsync(formFile, userId!);

        await _cvService.SaveCVAsync(parsedCV);
        return CreatedAtAction(nameof(GetCVs), parsedCV);
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