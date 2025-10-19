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
        
        builder.ComplexProperty(w => new {w.first_name, w.surname, w.second_name, w.birthday,}).IsRequired();
        
    }
}