using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkerService.DAL.Entities;

public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.HasKey(r => r.id);

        builder.Property(r => r.post)
            .IsRequired();

        builder.Property(r => r.skill)
            .IsRequired();

        builder.Property(r => r.city)
            .IsRequired();

        builder.Property(r => r.experience)
            .IsRequired();

        builder.Property(r => r.wantedSalary);

        builder.Property(r => r.income_date)
            .IsRequired();

        builder.Property(r => r.worker_id)
            .IsRequired();
        
        builder.HasOne(r => r.worker)
            .WithMany(w => w.Resumes)
            .HasForeignKey(r => r.worker_id)
            .OnDelete(DeleteBehavior.Cascade);

    }
}