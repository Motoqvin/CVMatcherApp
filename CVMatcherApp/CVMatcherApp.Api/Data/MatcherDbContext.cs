using CVMatcherApp.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CVMatcherApp.Api.Data;

public class MatcherDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public DbSet<AuditLog> Logs { get; set; }
    public DbSet<CV> CVs { get; set; }
    public DbSet<Result> Results { get; set; }
    public MatcherDbContext(DbContextOptions options) : base(options) { }
}