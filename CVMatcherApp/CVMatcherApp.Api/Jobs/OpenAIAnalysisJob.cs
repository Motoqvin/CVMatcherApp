using CVMatcherApp.Api.Services;
using CVMatcherApp.Api.Services.Base;

namespace CVMatcherApp.Api.Jobs;

public class OpenAIAnalysisJob
{
    private readonly IOpenAIService _openAIService;
    private readonly ICVService cVService;
    public OpenAIAnalysisJob(IOpenAIService openAIService, ICVService cVService)
    {
        _openAIService = openAIService;
        this.cVService = cVService;
    }

    public async Task AnalyzeCVAsync(int cvId)
    {
        var cv = await cVService.GetCVById(cvId);

        var analysisResult = await _openAIService.AnalyzeCV(cv);

        cv.Summary = analysisResult.Summary;
        cv.Suggestions = analysisResult.Suggestions;

        await cVService.UpdateCVAsync(cv); 
    }
}