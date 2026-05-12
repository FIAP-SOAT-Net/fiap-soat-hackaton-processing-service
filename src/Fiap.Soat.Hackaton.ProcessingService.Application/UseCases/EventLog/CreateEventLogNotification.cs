using MediatR;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.EventLog;

public record CreateEventLogNotification(Guid ProcessingFileId, string? StatusFrom, string StatusTo) : INotification;
