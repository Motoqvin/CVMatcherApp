using CVMatcherApp.Api.Data;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Repositories.Base;

namespace CVMatcherApp.Api.Repositories;

public class LogRepository : ILogRepository
{
    private readonly MatcherDbContext dbContext;

    public LogRepository(MatcherDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public Task InsertAsync(AuditLog log)
    {
        dbContext.Logs.Add(log);
        dbContext.SaveChanges();
        return Task.CompletedTask;
    }
}