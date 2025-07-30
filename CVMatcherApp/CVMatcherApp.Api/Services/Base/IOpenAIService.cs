using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Services.Base;
public interface IOpenAIService
{
    Task<Result> AnalyzeCV(CV cv);
}