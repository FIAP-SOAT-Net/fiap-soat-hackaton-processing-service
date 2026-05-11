using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Repositories;

public sealed class ProcessingFileRepository(AppDbContext appDbContext) : Repository<ProcessingFile>(appDbContext), IProcessingFileRepository
{
    public async Task<IReadOnlyList<ProcessingFile>> GetAllAsync(Guid? fileId, string? status, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
    {
        var query = Query();
        if (fileId is not null) query = query.Where(x => x.FileId == fileId);
        if (!string.IsNullOrEmpty(status)) query = query.Where(x => x.Status == status);
        if (startDate.HasValue) query = query.Where(x => x.CreatedAt >= startDate.Value);
        if (endDate.HasValue) query = query.Where(x => x.CreatedAt <= endDate.Value);

        return await query.ToListAsync(cancellationToken);
    }
}
