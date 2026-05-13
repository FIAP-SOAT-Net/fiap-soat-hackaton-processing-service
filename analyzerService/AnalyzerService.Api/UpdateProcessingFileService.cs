namespace AnalyzerService.Api;

public interface IUpdateProcessingFileService
{
    Task UpdateProcessingFileAsync(Guid id, string status);
}

public sealed class UpdateProcessingFileService(
    ILogger<UpdateProcessingFileService> logger,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory) : IUpdateProcessingFileService
{
    public async Task UpdateProcessingFileAsync(Guid id, string status)
    {
        logger.LogInformation("Updating processing file {Id}", id);
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(configuration["ProcessingService:BaseUrl"] ?? throw new InvalidOperationException("ProcessingService:BaseUrl not configured"));
        var body = new { Status = status };
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PatchAsync($"/processingFiles/{id}", content);
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Successfully updated processing file with id {Id}", id);
        }
        else
        {
            logger.LogError("Failed to update processing file with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
        }
    }
}
