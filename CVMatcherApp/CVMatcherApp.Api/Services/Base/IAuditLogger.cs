using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Services.Base;
public interface IAuditLogger
{
    Task LogAsync(AuditLog log);
}