using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Data;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Repositories;

public sealed class ProcessingEventLogRepository(AppDbContext appDbContext) : Repository<ProcessingEventLog>(appDbContext), IProcessingEventLogRepository
{
}