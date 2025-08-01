using CVMatcherApp.Api.Data;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Services;
using CVMatcherApp.Api.Services.Base;

namespace CVMatcherApp.Api.Jobs;

public class OpenAIAnalysisJob
{
    private readonly IOpenAIService _openAIService;
    private readonly MatcherDbContext dbContext;
    public OpenAIAnalysisJob(IOpenAIService openAIService, MatcherDbContext dbContext)
    {
        _openAIService = openAIService;
        this.dbContext = dbContext;
    }

    public async Task AnalyzeAndSaveCVAsync(string userId, CV cv)
    {
        var result = await _openAIService.AnalyzeCV(cv);

        var cvRecord = new CV
        {
            UserId = userId,
            Content = cv.Content,
            Summary = result.Summary,
            Suggestions = result.Suggestions,
            MatchScore = result.MatchScore,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.CVs.Add(cvRecord);
        await dbContext.SaveChangesAsync();
    }
}