using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using MediatR;
using System.Net;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Update;

public sealed class UpdateProcessingFileStatusHandler(IProcessingFileRepository repository) : IRequestHandler<UpdateProcessingFileStatusCommand, Response<ProcessingFile>>
{
    public async Task<Response<ProcessingFile>> Handle(UpdateProcessingFileStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return ResponseFactory.Fail<ProcessingFile>("Processing file not found", HttpStatusCode.NotFound);

        _ = entity.UpdateStatus(request.Status);
        return ResponseFactory.Ok(await repository.UpdateAsync(entity, cancellationToken));
    }
}
