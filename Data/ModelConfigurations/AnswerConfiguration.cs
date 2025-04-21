using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XForms.Data.ModelConfigurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        // Relationship
        builder.HasOne(a => a.Form)
            .WithMany(f => f.Answers)
            .HasForeignKey(f => f.FormId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // FastQuery
        builder.HasIndex(a => new { a.FormId, a.QuestionId });
        
        // Locking
        builder.Property(a => a.Version)
            .IsConcurrencyToken();
    }
}