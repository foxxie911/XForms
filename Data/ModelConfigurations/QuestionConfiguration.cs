using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XForms.Data.ModelConfigurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
   public void Configure(EntityTypeBuilder<Question> builder)
   {
       // Relationships
        builder.HasOne(q => q.Template)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Fast Query
        builder.HasIndex(q => new { q.TemplateId, q.Order });
        
        // Concurrency or Optimistic Locking
        builder.Property(q => q.Version).IsRowVersion();
   }
}