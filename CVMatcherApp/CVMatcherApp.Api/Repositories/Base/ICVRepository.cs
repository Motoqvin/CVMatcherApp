using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Repositories.Base;

public interface ICVRepository
{
    Task<CV> GetCVByIdAsync(int id);
    Task<List<CV>> GetAllCVsAsync();
    Task<int> SaveCVAsync(CV cv);
    Task<int> DeleteOldCVsAsync();
    Task<bool> UpdateCVAsync(CV cv);
    CV Extract(byte[] bytes);
}