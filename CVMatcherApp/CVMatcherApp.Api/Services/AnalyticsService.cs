using CVMatcherApp.Api.Data;
using CVMatcherApp.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CVMatcherApp.Api.Services;

public class AnalyticsService
{
    private readonly MatcherDbContext _dbContext;

    public AnalyticsService(MatcherDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserStatsDto>> GetUserStatsAsync()
    {
        var stats = _dbContext.Users
            .Select(user => new UserStatsDto
            {
                UserId = user.Id,
                Email = user.Email,
                TotalCVs = user.CVs!.Count(),
                AnalyzedCVs = user.CVs!.Count(cv => cv.IsAnalyzed),
                AverageMatchScore = user.CVs!
                    .Where(cv => cv.IsAnalyzed)
                    .Select(cv => (double?)cv.MatchScore)
                    .Average() ?? 0
            });

        return await stats.ToListAsync();
    }

    public async Task<UsageStatsDto> GetUsageStatsAsync()
    {
        var totalCVs = await _dbContext.CVs.CountAsync();
        var parsedCVs = await _dbContext.CVs.CountAsync(c => c.IsParsed);
        var analyzedCVs = await _dbContext.CVs.CountAsync(c => c.IsAnalyzed);
        var averageScore = await _dbContext.CVs
            .Where(c => c.IsAnalyzed)
            .Select(c => (double?)c.MatchScore)
            .AverageAsync() ?? 0;

        var latestUpload = await _dbContext.CVs
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => (DateTime?)c.CreatedAt)
            .FirstOrDefaultAsync();

        return new UsageStatsDto
        {
            TotalUsers = await _dbContext.Users.CountAsync(),
            TotalCVs = totalCVs,
            ParsedCVs = parsedCVs,
            AnalyzedCVs = analyzedCVs,
            AverageMatchScore = Math.Round(averageScore, 2),
            LatestCVUpload = latestUpload
        };
    }
}