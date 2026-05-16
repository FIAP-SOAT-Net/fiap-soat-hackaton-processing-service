using System.Text.Json.Serialization;

namespace AnalyzerService.Api;

public class RecommendationDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
