using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.DAL.Entities;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.UserId);
        builder.Property(c => c.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(c => c.name).IsRequired();
        builder.Property(c => c.longitude).IsRequired();
        builder.Property(c => c.latitude).IsRequired();
        builder.Property(c => c.email).IsRequired();
        builder.Property(c => c.phoneNumber).IsRequired();
        
        
    }
}