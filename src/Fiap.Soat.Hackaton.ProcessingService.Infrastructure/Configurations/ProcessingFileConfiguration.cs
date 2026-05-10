using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Configurations;

public sealed class ProcessingFileConfiguration : IEntityTypeConfiguration<ProcessingFile>
{
    public void Configure(EntityTypeBuilder<ProcessingFile> builder)
    {
        builder.ToTable("files", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "chk_files_status",
                "status IN ('RECEIVED', 'PROCESSING', 'ANALYZED', 'ERROR', 'PROCESSED')");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("VARCHAR(255)")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.BucketName)
            .HasColumnName("bucket_name")
            .HasColumnType("VARCHAR(255)")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Key)
            .HasColumnName("key")
            .HasColumnType("VARCHAR(1024)")
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.FileSizeBytes)
            .HasColumnName("file_size_bytes")
            .IsRequired();

        builder.Property(x => x.MimeType)
            .HasColumnName("mime_type")
            .HasColumnType("VARCHAR(100)")
            .HasMaxLength(100);

        builder.Property(x => x.UploadedAt)
            .HasColumnName("uploaded_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("VARCHAR(30)")
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.Status).HasDatabaseName("idx_files_status");
        builder.HasIndex(x => x.UploadedAt).HasDatabaseName("idx_files_uploaded_at");
        builder.HasIndex(x => new { x.BucketName, x.Key }).HasDatabaseName("idx_files_bucket_key");

        builder.HasMany(x => x.EventLogs)
            .WithOne(x => x.File)
            .HasForeignKey(x => x.FileId)
            .HasConstraintName("fk_event_logs_file_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
