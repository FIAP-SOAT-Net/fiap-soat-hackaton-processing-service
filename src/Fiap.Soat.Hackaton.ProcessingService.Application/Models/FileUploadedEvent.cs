using System.Text.Json.Serialization;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Models;

public sealed record FileUploadedEvent
{
    [JsonPropertyName("fileId")]
    public string FileId { get; init; }

    [JsonPropertyName("fileName")]
    public string FileName { get; init; } = string.Empty;

    [JsonPropertyName("contentType")]
    public string ContentType { get; init; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; init; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("bucketName")]
    public string BucketName { get; init; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; init; } = string.Empty;
}


