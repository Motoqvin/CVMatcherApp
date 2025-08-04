using System.ComponentModel.DataAnnotations;

namespace CVMatcherApp.Api.Models;

public class Result
{
    [Key]
    public int Id { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }

    public List<JobMatch> Matches { get; set; } = new List<JobMatch>();
}