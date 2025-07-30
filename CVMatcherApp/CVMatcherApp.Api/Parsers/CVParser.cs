// using System.Text.Json;
// using CVMatcherApp.Api.Models;
// using OpenAI;
// using OpenAI.Chat;

// namespace CVMatcherApp.Api.Parsers;

// public class CVParser
// {
//     private readonly OpenAIClient _client;
//     public CVParser(string apiKey)
//     {
//         _client = new OpenAIClient(apiKey);
//     }

//     public async Task<CV?> ParseCVAsync(string rawCvText)
//     {
//         var prompt = $"""
//         You are a CV/resume parser. Extract and return the following fields in a valid JSON object:

//         - FileName
//         - Content
//         - FullName
//         - Email
//         - PhoneNumber
//         - Location
//         - Education
//         - Experience
//         - Skills
//         - Summary
//         - Suggestions

//         Only return JSON. Do not include any explanation.

//         Resume Text:
//         {rawCvText}
//         """;

//         var request = new ChatCompletion(
//             Model: "gpt-4",
//             Messages: new[]
//             {
//                 new ChatMessage(Role.System, "You are a CV parsing assistant."),
//                 new ChatMessage(Role.User, prompt)
//             });

//         var response = await _client.ChatEndpoint.GetCompletionAsync(request);

//         var content = response.Choices.FirstOrDefault()?.Message.Content;

//         if (string.IsNullOrWhiteSpace(content))
//         {
//             Console.WriteLine("No response from OpenAI.");
//             return null;
//         }

//         try
//         {
//             var cv = JsonSerializer.Deserialize<CV>(content, new JsonSerializerOptions
//             {
//                 PropertyNameCaseInsensitive = true
//             });

//             if (cv != null)
//             {
//                 cv.Content = rawCvText;
//                 cv.CreatedAt = DateTime.UtcNow;
//                 cv.IsParsed = true;
//             }

//             return cv;
//         }
//         catch (JsonException ex)
//         {
//             Console.WriteLine("Error parsing OpenAI response: " + ex.Message);
//             Console.WriteLine("Raw response:\n" + content);
//             return null;
//         }
//     }
// }