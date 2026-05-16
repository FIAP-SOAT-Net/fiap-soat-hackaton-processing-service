namespace AnalyzerService.Api;

public static class MockAnalysisPayloadFactory
{
    public static AnalysisResultDto CreateSample(Guid id, string fileName)
    {
        return new AnalysisResultDto
        {
            AnalysisProcessId = id,
            SourceFileName = fileName,
            Components =
            [
                new ComponentDto { Name = "API Gateway", Type = "Gateway", Description = "Ponto de entrada único para requisições externas" },
                new ComponentDto { Name = "Auth Service", Type = "Service", Description = "Responsável por autenticação e autorização via JWT" }
            ],
            Risks =
            [
                new RiskDto
                {
                    Title = "Ponto único de falha",
                    Severity = "High",
                    Description = "API Gateway sem redundância pode causar indisponibilidade total",
                    Recommendation = "Adicionar réplicas com load balancer e health checks"
                },

                new RiskDto
                {
                    Title = "Ausência de rate limiting",
                    Severity = "Medium",
                    Description = "Endpoints expostos sem proteção contra abuso por volume",
                    Recommendation = "Configurar rate limiting no API Gateway"
                }
            ],
            Recommendations =
            [
                new RecommendationDto
                {
                    Title = "Observabilidade", Description = "Adicionar tracing distribuído com OpenTelemetry para rastreamento entre serviços"
                }
            ],
            AiModelInfo = new AiModelInfoDto { Provider = "OpenAI", Model = "gpt-4o", PromptVersion = "v1.0", Confidence = 0.88 }
        };
    }

    public static string CreateSampleJson(Guid id, string fileName)
    {
        var dto = CreateSample(id, fileName);
        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        return System.Text.Json.JsonSerializer.Serialize(dto, options);
    }
}
