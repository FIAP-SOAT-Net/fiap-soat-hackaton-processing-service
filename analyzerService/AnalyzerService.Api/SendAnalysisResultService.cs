namespace AnalyzerService.Api;

public interface ISendAnalysisResultService
{
    Task SendAsync(Guid id, string fileName);
}

public class SendAnalysisResultService(
    ILogger<UpdateProcessingFileService> logger,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory) : ISendAnalysisResultService
{
    public async Task SendAsync(Guid id, string fileName)
    {
        logger.LogInformation("Sending analysis from processing file {Id}", id);
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(configuration["ReportsService:BaseUrl"] ?? throw new InvalidOperationException("ReportsService:BaseUrl not configured"));
        var body = MockAnalysisPayloadFactory.CreateSample(id, fileName);
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"/api/reports", content);
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Successfully sent analysis from file id {Id}", id);
        }
        else
        {
            logger.LogError("Failed to sent analysis from file id {Id}. Status code: {StatusCode}", id, response.StatusCode);
        }
    }
}
