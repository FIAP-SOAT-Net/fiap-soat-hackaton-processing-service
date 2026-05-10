using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using MediatR;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.List;

public sealed class ListProcessingFilesHandler(IProcessingFileRepository repository) : IRequestHandler<ListProcessingFilesQuery, Response<IReadOnlyList<ProcessingFile>>>
{
    public async Task<Response<IReadOnlyList<ProcessingFile>>> Handle(ListProcessingFilesQuery request, CancellationToken cancellationToken)
    {
        var response = await repository.GetAllAsync(request.FileId, request.Status, request.StartDate, request.EndDate, cancellationToken);
        return ResponseFactory.Ok(response);
    }
}
