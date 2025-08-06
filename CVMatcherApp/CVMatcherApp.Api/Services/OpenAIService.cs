using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CVMatcherApp.Api.Dtos;
using CVMatcherApp.Api.Exceptions;
using CVMatcherApp.Api.Options;
using CVMatcherApp.Api.Repositories.Base;
using CVMatcherApp.Api.Services.Base;
using Microsoft.Extensions.Options;

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
        var prompt = $@"
You are a premium AI career strategist helping mission-driven professionals find aligned roles that match their talents, temperament, and trajectory.

Speak to the user as 'you' in {language} language and refer to the job as '{jobDescription}'.

Your tone is emotionally intelligent, practical, and encouraging — like a mentor with deep insight into vocational psychology, leadership energy, and personal growth.

GOAL:
Give a career compatibility reading that’s emotionally insightful, specific, and rooted in the candidate’s unique value — not generic fluff.

HOW:
- Compare the CV and job using:
  - Technical fit (skills & tools)
  - Career archetypes (e.g., strategist, builder, guide)
  - Workstyle alignment (e.g., autonomous vs. collaborative)
  - Passion alignment (e.g., hobbies, side projects, mission)

- One focused sentence per field.
- Mention at least one possible growth edge or mismatch (e.g., too visionary for a rigid environment).
- Use career psychology language (e.g., visionary thinker, systems builder, empathetic communicator).

DATA  
Candidate's CV is in JSON Format:
{cvText}

FORMAT:  
Return ONLY valid JSON in this structure, then add a one-line motivational wrap-up:

{{
  ""MatchScore"": number,
  ""Explanation"": ""One sentence about how your hard skills match — include one gap or surplus."",
  ""Suggestions"": ""One sentence giving several suggestions to the candidate to improve his/her cv in future""
}}

Respond ONLY with JSON:
""MatchScore"": number,
""Explanation"": ""string"",
""Suggestions"": ""string""


Hey — here’s the takeaway: <1 sentence pep talk in new wording — career-affirming, strengths-based, ends with a growth mindset>.
";

        var requestBody = new
        {
            model = _options.Model,
            messages = new[]
            {
                new { role = "system", content = "You are a professional CV compatibility analyst. Respond only with valid JSON." },
                new { role = "user", content = prompt }
            },
            temperature = 0.3,
            response_format = new { type = "json_object" },
            max_tokens = 1024
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        req.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        using var resp = await _httpClient.SendAsync(req);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(json).RootElement;

        var answerJson = root
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        var jsonOnly = Regex.Match(answerJson!, @"\{.*\}", RegexOptions.Singleline).Value;

        var match = JsonSerializer.Deserialize<JobMatchDto>(jsonOnly!, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        System.Console.WriteLine(match!.Suggestions);

        match!.JobDescription = jobDescription;

        return match!;
    }
}