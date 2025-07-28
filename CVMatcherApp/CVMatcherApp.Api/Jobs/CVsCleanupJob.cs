using CVMatcherApp.Api.Services.Base;

namespace CVMatcherApp.Api.Jobs;

public class CVsCleanupJob
{
    private readonly ICVService service;

    public CVsCleanupJob(ICVService service)
    {
        this.service = service;
    }

    public async Task Run()
    {
        var deleted = await service.CleanUpOldCVs();
    }
}