namespace Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

public sealed class ProcessingFile : Entity
{
    public string Name { get; private set; }
    public Guid FileId { get; private set; }
    public string BucketName { get; private set; }
    public string Key { get; private set; }
    public long FileSizeBytes { get; private set; }
    public string? MimeType { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string Status { get; private set; } = "RECEIVED";
    public ICollection<ProcessingEventLog> EventLogs { get; private set; } = new List<ProcessingEventLog>();

    public ProcessingFile(
        string name,
        string bucketName,
        string key,
        long fileSizeBytes,
        DateTime uploadedAt,
        string? mimeType = null,
        string status = "RECEIVED")
    {
        Name = name;
        BucketName = bucketName;
        Key = key;
        FileSizeBytes = fileSizeBytes;
        UploadedAt = uploadedAt;
        MimeType = mimeType;
        Status = status;
    }

    public ProcessingFile UpdateStatus(string status)
    {
        Status = status;
        MarkAsUpdated();
        return this;
    }

    public void AddEventLog(ProcessingEventLog eventLog)
    {
        EventLogs.Add(eventLog);
    }
}
