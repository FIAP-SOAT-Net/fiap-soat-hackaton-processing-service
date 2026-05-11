using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Configurations;

public sealed class ProcessingEventLogConfiguration : IEntityTypeConfiguration<ProcessingEventLog>
{
    public void Configure(EntityTypeBuilder<ProcessingEventLog> builder)
    {
        builder.ToTable("event_logs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(x => x.ProcessingFileId)
            .HasColumnName("processing_file_id")
            .HasColumnType("CHAR(36)")
            .IsRequired();

        builder.Property(x => x.EventType)
            .HasColumnName("event_type")
            .HasColumnType("VARCHAR(80)")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(x => x.StatusFrom)
            .HasColumnName("status_from")
            .HasColumnType("VARCHAR(30)")
            .HasMaxLength(30);

        builder.Property(x => x.StatusTo)
            .HasColumnName("status_to")
            .HasColumnType("VARCHAR(30)")
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.ProcessingFileId).HasDatabaseName("idx_event_logs_file_id");
        builder.HasIndex(x => x.EventType).HasDatabaseName("idx_event_logs_event_type");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_event_logs_created_at");

        builder.HasOne(x => x.File)
            .WithMany(x => x.EventLogs)
            .HasForeignKey(x => x.ProcessingFileId)
            .HasConstraintName("fk_event_logs_file_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
