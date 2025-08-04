using CVMatcherApp.Api.Dtos;

namespace CVMatcherApp.Api.Repositories.Base;
public interface IResultRepository
{
    Task SaveResultAsync(int resultId, List<JobMatchDto> results);
    Task<ResultDto?> GetResultAsync(int resultId);
    Task MarkAnalysisStartedAsync(int resultId);
}