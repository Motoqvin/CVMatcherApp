using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Options;
using CVMatcherApp.Api.Services.Base;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace CVMatcherApp.Api.Services;

public class OpenAIService : IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;

    public OpenAIService(HttpClient httpClient, IOptions<OpenAIOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);
    }

    public async Task<Result> AnalyzeCV(CV cv)
    {
        if (cv == null || string.IsNullOrEmpty(cv.Content))
        {
            throw new ArgumentException("CV content cannot be null or empty.");
        }

        var cvJson = JsonSerializer.Serialize(cv);

        string prompt = $@"
Analyze the following CV (in JSON format) and provide:
1. A summary of the candidate's strengths.
2. Suggestions for improving the CV.
3. A match score from 1 to 100 that reflects how strong this CV appears.

CV JSON:
{cvJson}
";
        ChatClient chatClient = new(model: "gpt-3.5-turbo", apiKey: _options.ApiKey);

        ChatCompletion chatCompletion = await chatClient.CompleteChatAsync(prompt);

        System.Console.WriteLine(chatCompletion.Content[0].Text);

        var result = new Result
        {

        };

        return result;
    }
}