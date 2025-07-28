using CVMatcherApp.Api.Repositories.Base;
using CVMatcherApp.Api.Services.Base;

namespace CVMatcherApp.Api.Services;

public class CVService : ICVService
{
    private readonly ICVRepository repository;

    public CVService(ICVRepository repository)
    {
        this.repository = repository;
    }
    public async Task<int> CleanUpOldCVs()
    {
        return await repository.DeleteOldCVsAsync();
    }
}