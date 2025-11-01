using FeedbackService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeedbackService.DAL.Data.TypeConfiguration;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.HasOne(f => f.resume)
            .WithMany(r => r.feedbacks)
            .HasForeignKey(f => f.resumeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(f => f.vacancy)
            .WithMany(r => r.feedbacks)
            .HasForeignKey(f => f.vacancyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}