using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Repositories.Base;
using CVMatcherApp.Api.Services.Base;

namespace CVMatcherApp.Api.Services;

public class CVService : ICVService
{
    private readonly ICVRepository repository;
    private readonly IOpenAIService openAIService;

    public CVService(ICVRepository repository, IOpenAIService openAIService)
    {
        this.repository = repository;
        this.openAIService = openAIService; 
    }

    public async Task<Result> AnalyzeCVAsync(int cvId)
    {
        return await openAIService.AnalyzeCV(await GetCVById(cvId));
    }

    public async Task<int> CleanUpOldCVs()
    {
        return await repository.DeleteOldCVsAsync();
    }

    public async Task<List<CV>> GetAllCVsAsync(string userId)
    {
        return await repository.GetAllCVsAsync(userId);
    }

    public async Task<CV> GetCVById(int id)
    {
        return await repository.GetCVByIdAsync(id);
    }


    public CV Parse(string text)
    {
        var cv = new CV();

        var nameMatch = Regex.Match(text, @"(?i)(Name|Full Name):\s*(.+)");
        if (nameMatch.Success)
            cv.FullName = nameMatch.Groups[2].Value.Trim();

        var emailMatch = Regex.Match(text, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-z]{2,}");
        if (emailMatch.Success)
            cv.Email = emailMatch.Value;

        var phoneMatch = Regex.Match(text, @"(\+?\d[\d\s\-]{7,})");
        if (phoneMatch.Success)
            cv.PhoneNumber = phoneMatch.Value;

        var locationMatch = Regex.Match(text, @"(?i)(Location|Address):\s*(.+)");
        if (locationMatch.Success)
            cv.Location = locationMatch.Groups[2].Value.Trim();

        var educationMatch = Regex.Match(text, @"(?i)(Education|Qualifications)([\s\S]*?)(?=Work Experience|Skills|$)");
        if (educationMatch.Success)
            cv.Education = educationMatch.Groups[2].Value.Trim();

        var experienceMatch = Regex.Match(text, @"(?i)(Work Experience|Experience)([\s\S]*?)(?=Education|Skills|$)");
        if (experienceMatch.Success)
            cv.Experience = experienceMatch.Groups[2].Value.Trim();

        var skillsMatch = Regex.Match(text, @"(?i)(Skills)([\s\S]*?)(?=Experience|Education|$)");
        if (skillsMatch.Success)
            cv.Skills = skillsMatch.Groups[2].Value.Trim();

        return cv;
    }

    public async Task<bool> SaveCVAsync(IFormFile file, string userId)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("File cannot be null or empty");

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".pdf" && extension != ".docx") throw new ArgumentException("Unsupported file type");

        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        var fileContent = memoryStream.ToArray();
        CV cv;

        cv = extension == ".pdf" ? repository.ExtractPdf(fileContent) : repository.ExtractDocx(fileContent);

        cv = Parse(cv.Content!);
        cv.UserId = userId;
        cv.IsParsed = true;
        cv.FileName = file.FileName;
        cv.Content = Convert.ToBase64String(fileContent);

        await repository.SaveCVAsync(cv);
        return true;
    }

    public async Task<bool> UpdateCVAsync(CV cv)
    {
        ArgumentNullException.ThrowIfNull(cv);

        if (string.IsNullOrEmpty(cv.FileName))
            throw new ArgumentException("FileName cannot be null or empty");

        if (string.IsNullOrEmpty(cv.Content))
            throw new ArgumentException("Content cannot be null or empty");

        return await repository.UpdateCVAsync(cv);
    }
}