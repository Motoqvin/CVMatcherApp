using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CVMatcherApp.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CVController : ControllerBase
{
    [HttpGet]
    public IActionResult GetCVs()
    {
        return Ok(new { Message = "List of CVs" });
    }

    [HttpPost("/upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCV([FromForm] IFormFile formFile)
    {
        if (formFile == null || formFile.Length == 0) return BadRequest();
        var extension = Path.GetExtension(formFile.FileName).ToLower();

        if (extension != "pdf" || extension != "docx") return BadRequest();

        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        var fileBytes = memoryStream.ToArray();
        return CreatedAtAction(nameof(GetCVs), new { Message = "CV created successfully" });
    }
}