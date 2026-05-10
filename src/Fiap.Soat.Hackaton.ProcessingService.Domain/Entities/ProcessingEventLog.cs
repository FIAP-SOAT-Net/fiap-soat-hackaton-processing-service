namespace Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

public sealed class ProcessingEventLog : Entity
{
    public Guid FileId { get; private set; }
    public string EventType { get; private set; }
    public string? StatusFrom { get; private set; }
    public string StatusTo { get; private set; }
    public ProcessingFile File { get; private set; } = null!;

    private ProcessingEventLog()
    {
        EventType = string.Empty;
        StatusTo = string.Empty;
    }

    public ProcessingEventLog(Guid fileId, string eventType, string? statusFrom, string statusTo)
    {
        FileId = fileId;
        EventType = eventType;
        StatusFrom = statusFrom;
        StatusTo = statusTo;
    }
}