using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVMatcherApp.Api.Models;

public class JobMatch
{
    [Key]
    public int Id { get; set; }
    public string JobDescription { get; set; } = string.Empty;
    public int MatchScore { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public string Suggestions { get; set; } = string.Empty;
}