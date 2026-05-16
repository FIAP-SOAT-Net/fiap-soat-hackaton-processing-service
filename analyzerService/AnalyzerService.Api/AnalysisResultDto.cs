using System.Text.Json.Serialization;

namespace AnalyzerService.Api;

public class AnalysisResultDto
{
    [JsonPropertyName("analysisProcessId")]
    public Guid AnalysisProcessId { get; set; }

    [JsonPropertyName("sourceFileName")]
    public string SourceFileName { get; set; } = string.Empty;

    [JsonPropertyName("components")]
    public List<ComponentDto> Components { get; set; } = [];

    [JsonPropertyName("risks")]
    public List<RiskDto> Risks { get; set; } = [];

    [JsonPropertyName("recommendations")]
    public List<RecommendationDto> Recommendations { get; set; } = [];

    [JsonPropertyName("aiModelInfo")]
    public AiModelInfoDto? AiModelInfo { get; set; }
}
