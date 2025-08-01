namespace CVMatcherApp.Api.Dtos;

public class UsageStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalCVs { get; set; }
    public int ParsedCVs { get; set; }
    public int AnalyzedCVs { get; set; }
    public double AverageMatchScore { get; set; }
    public DateTime? LatestCVUpload { get; set; }
}