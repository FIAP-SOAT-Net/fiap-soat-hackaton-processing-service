using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using MediatR;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Update;

public record UpdateProcessingFileStatusCommand(Guid Id, string Status) : IRequest<Response<ProcessingFile>>;
