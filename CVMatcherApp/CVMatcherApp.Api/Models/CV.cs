using System.ComponentModel.DataAnnotations.Schema;

namespace CVMatcherApp.Api.Models;

public class CV
{
    public int Id { get; set; }
    public string? FileName { get; set; }
    public string? Content { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public string? Education { get; set; }
    public string? Experience { get; set; }
    public string? Skills { get; set; }
    public string? Summary { get; set; }
    public string? Suggestions { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsParsed { get; set; }
    public bool IsAnalyzed { get; set; }
    public int MatchScore { get; set; }

    public int ResultId { get; set; }
    [ForeignKey(nameof(ResultId))]
    public Result AnalysisResult { get; set; } = new Result();
}