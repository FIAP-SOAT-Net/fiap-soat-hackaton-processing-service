namespace Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

public sealed class ProcessingEventLog : Entity
{
    public Guid ProcessingProcessingFileId { get; private set; }
    public string EventType { get; private set; }
    public string? StatusFrom { get; private set; }
    public string StatusTo { get; private set; }

    private ProcessingEventLog() { }

    public ProcessingEventLog(Guid processingFileId, string eventType, string? statusFrom, string statusTo)
    {
        ProcessingProcessingFileId = processingFileId;
        EventType = eventType;
        StatusFrom = statusFrom;
        StatusTo = statusTo;
    }
}
