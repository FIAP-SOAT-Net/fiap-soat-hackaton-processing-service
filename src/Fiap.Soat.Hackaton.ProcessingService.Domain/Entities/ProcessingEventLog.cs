namespace Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

public sealed class ProcessingEventLog : Entity
{
    public Guid ProcessingFileId { get; private set; }
    public string EventType { get; private set; }
    public string? StatusFrom { get; private set; }
    public string StatusTo { get; private set; }

    private ProcessingEventLog() { }

    private ProcessingEventLog(Guid processingFileId, string eventType, string? statusFrom, string statusTo)
    {
        ProcessingFileId = processingFileId;
        EventType = eventType;
        StatusFrom = statusFrom;
        StatusTo = statusTo;
    }

    public static ProcessingEventLog Create(Guid processingFileId, string eventType, string? statusFrom, string statusTo) =>
        new ProcessingEventLog(processingFileId, eventType, statusFrom, statusTo);
}
