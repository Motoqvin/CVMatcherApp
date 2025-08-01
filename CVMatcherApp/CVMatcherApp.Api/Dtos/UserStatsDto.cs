namespace CVMatcherApp.Api.Dtos;
public class UserStatsDto
{
    public string? UserId { get; set; }
    public string? Email { get; set; }

    public int TotalCVs { get; set; }
    public int AnalyzedCVs { get; set; }
    public double AverageMatchScore { get; set; }
}