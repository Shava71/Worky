using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.DAL.Entities.TypeConfiguration;

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("Deals");

        builder.HasKey(d => d.id);

        builder.Property(d => d.id)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.tariff_id)
            .IsRequired();

        builder.Property(d => d.company_id)
            .IsRequired(false);

        // builder.Property(d => d.status)
        //     .IsRequired()
        //     .HasDefaultValue(false);

        builder.Property(d => d.date_start)
            .IsRequired();

        builder.Property(d => d.date_end)
            .IsRequired();

        builder.Property(d => d.sum)
            .IsRequired();
        
        builder.HasOne(d => d.tariff)
            .WithMany() 
            .HasForeignKey(d => d.tariff_id)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(d => d.company)
            .WithMany(c => c.Deals)
            .HasForeignKey(d => d.company_id)
            .OnDelete(DeleteBehavior.SetNull);
    }
}