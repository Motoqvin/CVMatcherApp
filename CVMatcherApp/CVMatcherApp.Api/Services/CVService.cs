using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Repositories.Base;
using CVMatcherApp.Api.Services.Base;

namespace CVMatcherApp.Api.Services;

public class CVService : ICVService
{
    private readonly ICVRepository repository;
    private readonly IOpenAIService openAIService;

    public CVService(ICVRepository repository, IOpenAIService openAIService)
    {
        this.repository = repository;
        this.openAIService = openAIService; 
    }

    public async Task<Result> AnalyzeCVAsync(int cvId)
    {
        return await openAIService.AnalyzeCV(await GetCVById(cvId));
    }

    public async Task<int> CleanUpOldCVs()
    {
        return await repository.DeleteOldCVsAsync();
    }

    public async Task<List<CV>> GetAllCVsAsync(string userId)
    {
        return await repository.GetAllCVsAsync(userId);
    }

    public async Task<CV> GetCVById(int id) => await repository.GetCVByIdAsync(id);


    public async Task<bool> SaveCVAsync(CV cv) => await repository.SaveCVAsync(cv) > 0;

    public async Task<bool> UpdateCVAsync(CV cv)
    {
        ArgumentNullException.ThrowIfNull(cv);

        if (string.IsNullOrEmpty(cv.FileName))
            throw new ArgumentNullException("FileName cannot be null or empty");

        if (string.IsNullOrEmpty(cv.Content))
            throw new ArgumentNullException("Content cannot be null or empty");

        return await repository.UpdateCVAsync(cv);
    }
}