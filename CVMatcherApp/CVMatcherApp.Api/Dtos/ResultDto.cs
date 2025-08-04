using CVMatcherApp.Api.Models;

namespace CVMatcherApp.Api.Dtos;
public class ResultDto
{
    public int ResultId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<JobMatch> Matches { get; set; } = new();
}