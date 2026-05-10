using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using MediatR;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Get;

public record GetProcessingFileByIdQuery(Guid Id) : IRequest<Response<ProcessingFile>>;
