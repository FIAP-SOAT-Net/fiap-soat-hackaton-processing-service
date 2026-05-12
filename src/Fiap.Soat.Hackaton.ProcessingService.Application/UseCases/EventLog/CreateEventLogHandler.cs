using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using MediatR;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.EventLog;

public sealed class CreateEventLogHandler(IProcessingEventLogRepository repository) : INotificationHandler<CreateEventLogNotification>
{
    private const string StatusReceived = "RECEIVED";
    private const string StatusProcessing = "PROCESSING";
    private const string StatusAnalyzed = "ANALYZED";
    private const string StatusProcessed = "PROCESSED";
    private const string StatusError = "ERROR";

    private const string EventFileReceived = "FILE_RECEIVED";
    private const string EventProcessingStarted = "PROCESSING_STARTED";
    private const string EventExternalRequestSent = "EXTERNAL_REQUEST_SENT";
    private const string EventAnalysisCompleted = "ANALYSIS_COMPLETED";
    private const string EventProcessingFinalized = "PROCESSING_FINALIZED";
    private const string EventProcessingFailed = "PROCESSING_FAILED";
    private const string EventFileStatusUpdated = "FILE_STATUS_UPDATED";

    public async Task Handle(CreateEventLogNotification notification, CancellationToken cancellationToken)
    {
        string eventType = ResolveEventType(notification.StatusFrom, notification.StatusTo);
        var entity = ProcessingEventLog.Create(notification.ProcessingFileId, eventType, notification.StatusFrom, notification.StatusTo);
        _ = await repository.AddAsync(entity, cancellationToken);
    }

    private static string ResolveEventType(string? statusFrom, string statusTo) => (statusFrom, statusTo) switch
    {
        (null, StatusReceived) => EventFileReceived,
        (StatusReceived, StatusProcessing) => EventProcessingStarted,
        (StatusProcessing, StatusProcessing) => EventExternalRequestSent,
        (StatusProcessing, StatusAnalyzed) => EventAnalysisCompleted,
        (StatusAnalyzed, StatusProcessed) => EventProcessingFinalized,
        (_, StatusError) => EventProcessingFailed,
        _ => EventFileStatusUpdated,
    };
}
