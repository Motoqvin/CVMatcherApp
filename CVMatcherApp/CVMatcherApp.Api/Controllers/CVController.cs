using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using CVMatcherApp.Api.Dtos;
using CVMatcherApp.Api.Jobs;
using CVMatcherApp.Api.Repositories.Base;
using CVMatcherApp.Api.Services;
using CVMatcherApp.Api.Services.Base;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVMatcherApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]
public class CVController : ControllerBase
{
    private readonly ICVService _cvService;
    private readonly IResultRepository _resultRepo;
    private readonly CVParserService parserService;
    public CVController(ICVService cvService, CVParserService parserService, IResultRepository resultRepository)
    {
        _cvService = cvService;
        this.parserService = parserService;
        _resultRepo = resultRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetCVs()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        var cvs = await _cvService.GetAllCVsAsync(userId);
        return Ok(cvs);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCVAsync(IFormFile formFile)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        var parsedCV = await parserService.ParseCVAsync(formFile, userId);

        await _cvService.SaveCVAsync(parsedCV);
        return CreatedAtAction(nameof(GetCVs), parsedCV);
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeCV([FromBody] AnalysisRequestDto requestDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        var cv = await _cvService.GetCVById(requestDto.CVId);
        BackgroundJob.Enqueue<OpenAIAnalysisJob>(job => job.AnalyzeAndSaveCVAsync(requestDto.CVId, requestDto.JobDescriptions!, cv.ResultId, requestDto.Language!));

        return Ok(new { Message = "CV analysis started successfully" });
    }

    [HttpGet("result/{resultId:int}")]
    public async Task<IActionResult> GetResult(int resultId)
    {
        var result = await _resultRepo.GetResultAsync(resultId);
        if (result == null)
            return NotFound(new { message = "Result not found" });

        return Ok(result);
    }
}