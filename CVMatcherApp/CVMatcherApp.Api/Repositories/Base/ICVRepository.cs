using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Repositories.Base;

public interface ICVRepository
{
    Task<int> DeleteOldCVsAsync();
    CV ParseCV(string file);
}