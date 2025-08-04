using CVMatcherApp.Api.Data;
using CVMatcherApp.Api.Dtos;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CVMatcherApp.Api.Repositories;

public class ResultRepository : IResultRepository
{
    private readonly MatcherDbContext dbContext;

    public ResultRepository(MatcherDbContext dbContext)
    {
        this.dbContext = dbContext;
    }


    public async Task<ResultDto?> GetResultAsync(int resultId)
    {
        var entity = await dbContext.Results
            .Include(a => a.Matches)
            .FirstOrDefaultAsync(a => a.Id == resultId);

        if (entity == null)
            return null;

        return new ResultDto
        {
            ResultId = entity.Id,
            StartedAt = entity.StartedAt,
            CompletedAt = entity.CompletedAt,
            IsCompleted = entity.IsCompleted,
            Matches = entity.Matches.Select(m => new JobMatch
            {
                JobDescription = m.JobDescription,
                MatchScore = m.MatchScore,
                Explanation = m.Explanation
            }).ToList()
        };
    }

    public async Task MarkAnalysisStartedAsync(int resultId)
    {
        var analysis = new Result
        {
            Id = resultId,
            StartedAt = DateTime.UtcNow,
            IsCompleted = false
        };

        dbContext.Results.Update(analysis);
        await dbContext.SaveChangesAsync();
    }


    public async Task SaveResultAsync(int resultId, List<JobMatchDto> results)
    {
        var analysis = await dbContext.Results
            .Include(a => a.Matches)
            .FirstOrDefaultAsync(a => a.Id == resultId);

        if (analysis == null)
            throw new Exception("Analysis result not found.");

        foreach (var match in results)
        {
            analysis.Matches.Add(new JobMatch
            {
                JobDescription = match.JobDescription,
                MatchScore = match.MatchScore,
                Explanation = match.Explanation,
            });
        }

        analysis.IsCompleted = true;
        analysis.CompletedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }
}