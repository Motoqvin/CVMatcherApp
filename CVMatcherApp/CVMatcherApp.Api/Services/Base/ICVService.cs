using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Services.Base;

public interface ICVService
{
    Task<int> CleanUpOldCVs();
    void SaveCVAsync(IFormFile file);
    Task<CV> GetCVById(int id);
    Task<List<CV>> GetAllCVsAsync();
    Task<bool> UpdateCVAsync(CV cv);
    Task<Result> AnalyzeCVAsync(int cvId);
}