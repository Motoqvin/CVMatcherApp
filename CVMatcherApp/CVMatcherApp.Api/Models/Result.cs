namespace CVMatcherApp.Api.Models;

public class Result
{
    public int Id { get; set; }
    public int MatchScore { get; set; }
    public string? Summary { get; set; }
    public string? Suggestions { get; set; }
}