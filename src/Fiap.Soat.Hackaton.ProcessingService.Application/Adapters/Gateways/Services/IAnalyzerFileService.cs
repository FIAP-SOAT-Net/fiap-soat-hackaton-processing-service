using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Services;

public interface IAnalyzerFileService
{
    Task SendAsync(ProcessingFile processingFile);
}
