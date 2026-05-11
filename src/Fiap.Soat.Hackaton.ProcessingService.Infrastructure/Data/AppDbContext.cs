using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Data;

[ExcludeFromCodeCoverage]
public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
     public DbSet<ProcessingFile> ProcessingFiles { get; set; }
     public DbSet<ProcessingEventLog> EventLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ProcessingFileConfiguration());
        modelBuilder.ApplyConfiguration(new ProcessingEventLogConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(Console.WriteLine).EnableSensitiveDataLogging();
}
