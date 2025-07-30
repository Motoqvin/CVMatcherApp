using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Options;
using CVMatcherApp.Api.Services.Base;
using Microsoft.Extensions.Options;

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
            var text = JsonSerializer.Serialize(cv);
            var prompt = $"Analyze this CV text and return:\n1. A summary of strengths\n2. Suggestions to improve it.\n3.Match Score for this specific person\n\n{text}";

            var requestBody = new
            {
                model = _options.Model,
                messages = new[]
                {
                    new { role = "system", content = "You are a CV analysis assistant." },
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var res = await JsonDocument.ParseAsync(stream);

            var reply = res.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
            if (string.IsNullOrEmpty(reply))
            {
                throw new Exception("OpenAI response is empty or invalid.");
            }
            var parts = reply.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
            {
                throw new Exception("OpenAI response does not contain expected parts.");
            }
            var summary = parts[0].Trim();
            var suggestions = parts[1].Trim();
            if (!int.TryParse(parts[2].Trim(), out var matchScore))
            {
                throw new Exception("Match Score is not a valid integer.");
            }
            var result = new Result
            {
                Summary = summary,
                Suggestions = suggestions,
                MatchScore = matchScore
            };
        return result;
        }
    }