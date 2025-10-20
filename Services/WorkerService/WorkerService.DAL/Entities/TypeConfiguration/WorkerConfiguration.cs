using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkerService.DAL.Entities;

public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder.HasKey(w => w.UserId);
        builder.Property(w => w.UserId)
            .HasMaxLength(450)
            .IsRequired();
        
        builder.Property(w => w.first_name).IsRequired();
        builder.Property(w => w.surname).IsRequired();
        builder.Property(w => w.second_name).IsRequired();
        builder.Property(w => w.birthday).IsRequired();
        
    }
}