using System.Text.Json.Serialization;

namespace AnalyzerService.Api;

public class RiskDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("recommendation")]
    public string? Recommendation { get; set; }
}
