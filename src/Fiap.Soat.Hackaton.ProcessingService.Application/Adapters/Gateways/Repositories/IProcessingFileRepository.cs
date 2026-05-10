using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;

public interface IProcessingFileRepository : IRepository<ProcessingFile>
{
    Task<IReadOnlyList<ProcessingFile>> GetAllAsync(string? fileId, string? status, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken);
}
