using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Presenters;
using Fiap.Soat.Hackaton.ProcessingService.Application.Mappers;
using Fiap.Soat.Hackaton.ProcessingService.Application.Models;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Get;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.List;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Controllers;

public sealed class ProcessingFilesController(IMediator mediator) : IProcessingFilesController
{
    public async Task<ActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProcessingFileByIdQuery(id), cancellationToken);
        var result = ResponseMapper.Map(response, ProcessingFilePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }

    public async Task<ActionResult> GetAllAsync(GetProcessingFileQuery request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ListProcessingFilesQuery(request.FileId, request.Status, request.StartDate, request.EndDate), cancellationToken);
        var result = ResponseMapper.Map(response, ProcessingFilePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }

    public async Task<IActionResult> UpdateAsync(Guid id, UpdateFileProcessingRequest request, CancellationToken cancellationToken)
    {
        UpdateProcessingFileStatusCommand command = new(id, request.Status);
        var response = await mediator.Send(command, cancellationToken);
        var result = ResponseMapper.Map(response, ProcessingFilePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }
}
