using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Services.Base;

public interface ICVService
{
    Task<int> CleanUpOldCVs();
    Task<bool> SaveCVAsync(IFormFile file, string userId);
    Task<CV> GetCVById(int id);
    Task<List<CV>> GetAllCVsAsync(string userId);
    Task<bool> UpdateCVAsync(CV cv);
    Task<Result> AnalyzeCVAsync(int cvId);
}