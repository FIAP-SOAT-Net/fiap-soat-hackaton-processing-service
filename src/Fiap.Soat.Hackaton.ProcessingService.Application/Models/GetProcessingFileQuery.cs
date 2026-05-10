namespace Fiap.Soat.Hackaton.ProcessingService.Application.Models;

public record GetProcessingFileQuery(string? FileId, string? Status, DateTime? StartDate, DateTime? EndDate);
