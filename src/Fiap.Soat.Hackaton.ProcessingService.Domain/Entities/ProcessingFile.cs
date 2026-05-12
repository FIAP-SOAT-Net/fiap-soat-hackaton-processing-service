namespace Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

public sealed class ProcessingFile : Entity
{
    public string Name { get; private set; }
    public string FileId { get; private set; }
    public string BucketName { get; private set; }
    public string Key { get; private set; }
    public long FileSizeBytes { get; private set; }
    public string? MimeType { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string Status { get; private set; } = "RECEIVED";

    private ProcessingFile() { }

    public ProcessingFile(
        string fileId,
        string name,
        string bucketName,
        string key,
        long fileSizeBytes,
        DateTime uploadedAt,
        string? mimeType)
    {
        FileId = fileId;
        Name = name;
        BucketName = bucketName;
        Key = key;
        FileSizeBytes = fileSizeBytes;
        UploadedAt = uploadedAt;
        MimeType = mimeType;
    }

    public ProcessingFile UpdateStatus(string status)
    {
        Status = status;
        MarkAsUpdated();
        return this;
    }
}
