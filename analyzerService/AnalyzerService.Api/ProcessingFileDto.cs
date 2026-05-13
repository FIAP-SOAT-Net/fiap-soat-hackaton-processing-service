namespace AnalyzerService.Api;

public record ProcessingFileDto(
    Guid Id,
    string Name,
    string FileId,
    string BucketName,
    string Key,
    long FileSizeBytes,
    string? MimeType,
    DateTime UploadedAt,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
