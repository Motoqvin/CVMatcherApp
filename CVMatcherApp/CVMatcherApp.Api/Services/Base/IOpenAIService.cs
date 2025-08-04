using CVMatcherApp.Api.Dtos;
using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Services.Base;

public interface IOpenAIService
{
    Task ProcessAnalysisAsync(int analyzeId, List<string> jobDescriptions, int resultId, string language);
    Task<JobMatchDto> AnalyzeCvAgainstJobAsync(string cvText, string jobDescription, string language);
}