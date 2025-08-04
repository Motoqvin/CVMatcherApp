namespace CVMatcherApp.Api.Dtos;
public class JobMatchDto
{
    public string JobDescription { get; set; } = string.Empty;
    public string Suggestions { get; set; } = string.Empty;
    public int MatchScore { get; set; }
    public string Explanation { get; set; } = string.Empty;
}