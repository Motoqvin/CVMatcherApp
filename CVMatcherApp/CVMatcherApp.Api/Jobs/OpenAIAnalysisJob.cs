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

    public async Task AnalyzeAndSaveCVAsync(int cvId, List<string> jobDescriptions, int resultId, string language)
    {
        await _openAIService.ProcessAnalysisAsync(cvId, jobDescriptions, resultId, language);
    }
}