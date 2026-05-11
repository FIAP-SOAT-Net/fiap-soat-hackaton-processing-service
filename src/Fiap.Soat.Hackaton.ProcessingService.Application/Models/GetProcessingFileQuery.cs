namespace Fiap.Soat.Hackaton.ProcessingService.Application.Models;

public record GetProcessingFileQuery(Guid? FileId, string? Status, DateTime? StartDate, DateTime? EndDate);
