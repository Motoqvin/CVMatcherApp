namespace CVMatcherApp.Api.Dtos;

public class AnalysisRequestDto
{
    public int CVId { get; set; }
    public List<string>? JobDescriptions { get; set; }
    public string? Language { get; set; }
}