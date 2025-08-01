using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Repositories.Base;

public interface ICVRepository
{
    Task<CV> GetCVByIdAsync(int id);
    Task<List<CV>> GetAllCVsAsync(string userId);
    Task<int> SaveCVAsync(CV cv);
    Task<int> DeleteOldCVsAsync();
    Task<bool> UpdateCVAsync(CV cv);
    CV ExtractPdf(byte[] bytes);
    CV ExtractDocx(byte[] bytes);
}