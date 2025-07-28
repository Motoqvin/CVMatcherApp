using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Repositories.Base;
public interface ILogRepository
{
    Task InsertAsync(AuditLog log);
}