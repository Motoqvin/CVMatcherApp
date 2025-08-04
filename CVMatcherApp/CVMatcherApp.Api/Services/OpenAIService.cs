using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CVMatcherApp.Api.Dtos;
using CVMatcherApp.Api.Exceptions;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Options;
using CVMatcherApp.Api.Repositories.Base;
using CVMatcherApp.Api.Services.Base;
using Hangfire.Common;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace CVMatcherApp.Api.Services;

public class OpenAIService : IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;
    private readonly ICVRepository _cvRepo;
    private readonly IResultRepository _resultRepo;

    public OpenAIService(HttpClient httpClient, IOptions<OpenAIOptions> options, ICVRepository cvRepository, IResultRepository resultRepository)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _cvRepo = cvRepository;
        _resultRepo = resultRepository;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);
    }

    public async Task ProcessAnalysisAsync(int analyzeId, List<string> jobDescriptions, int resultId, string language)
    {
        var cv = await _cvRepo.GetCVByIdAsync(analyzeId) ?? throw new NotFoundException();
        var analysisResults = new List<JobMatchDto>();
        
        await _resultRepo.MarkAnalysisStartedAsync(cv.Id);

        foreach (var job in jobDescriptions)
        {
            var result = await AnalyzeCvAgainstJobAsync(JsonSerializer.Serialize(cv), job, language);
            analysisResults.Add(result);
        }

        await _resultRepo.SaveResultAsync(resultId, analysisResults);
    }


    public async Task<JobMatchDto> AnalyzeCvAgainstJobAsync(string cvText, string jobDescription, string language)
    {
        var prompt = $"Compare this CV to the job description and rate the match from 0 to 100 in {language} language. Explain why.\n\nCV:\n{cvText}\n\nJob:\n{jobDescription}";

        var response = await _httpClient.PostAsJsonAsync("openai-api-url", new { prompt });
        var result = await response.Content.ReadFromJsonAsync<JobMatchDto>();

        return result!;
    }
}