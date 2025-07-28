using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Repositories.Base;
using CVMatcherApp.Api.Services.Base;

namespace CVMatcherApp.Api.Services;

public class AuditLogger : IAuditLogger
{
    private readonly ILogRepository logRepository;

    public AuditLogger(ILogRepository logRepository)
    {
        this.logRepository = logRepository;
    }

    public async Task LogAsync(AuditLog log)
    {
        if (log == null)
        {
            throw new ArgumentNullException(nameof(log), "Log cannot be null");
        }

        await logRepository.InsertAsync(log);
    }
}