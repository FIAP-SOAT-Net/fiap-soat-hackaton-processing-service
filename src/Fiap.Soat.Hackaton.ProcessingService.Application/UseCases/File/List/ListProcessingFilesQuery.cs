using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using MediatR;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.List;

public record ListProcessingFilesQuery(Guid? FileId, string? Status, DateTime? StartDate, DateTime? EndDate) : IRequest<Response<IReadOnlyList<ProcessingFile>>>;
