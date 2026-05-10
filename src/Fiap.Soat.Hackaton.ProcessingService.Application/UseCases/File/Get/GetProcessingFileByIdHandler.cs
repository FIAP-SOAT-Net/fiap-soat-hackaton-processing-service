using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using MediatR;
using System.Net;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Get;

public sealed class GetProcessingFileByIdHandler(IProcessingFileRepository repository) : IRequestHandler<GetProcessingFileByIdQuery, Response<ProcessingFile>>
{
    public async Task<Response<ProcessingFile>> Handle(GetProcessingFileByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity != null
            ? ResponseFactory.Ok(entity)
            : ResponseFactory.Fail<ProcessingFile>("Processing file Not Found", HttpStatusCode.NotFound);
    }
}
