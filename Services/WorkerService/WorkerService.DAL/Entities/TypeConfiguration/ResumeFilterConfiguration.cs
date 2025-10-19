using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkerService.DAL.Entities;

public class ResumeFilterConfiguration : IEntityTypeConfiguration<Resume_filter>
{
    public void Configure(EntityTypeBuilder<Resume_filter> builder)
    {
        builder.HasKey(f => f.filter_id);

        builder.Property(f => f.resume_id)
            .IsRequired();

        builder.Property(f => f.typeOfActivity_id)
            .IsRequired();

        builder.HasIndex(f => new { f.resume_id, f.typeOfActivity_id })
            .IsUnique();

        builder.HasOne(f => f.resume)
            .WithMany(r => r.resume_filters)
            .HasForeignKey(f => f.resume_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}