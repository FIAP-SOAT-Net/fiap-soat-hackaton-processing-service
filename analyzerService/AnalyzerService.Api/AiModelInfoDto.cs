using System.Text.Json.Serialization;

namespace AnalyzerService.Api;

public class AiModelInfoDto
{
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("promptVersion")]
    public string? PromptVersion { get; set; }

    [JsonPropertyName("confidence")]
    public double? Confidence { get; set; }
}
