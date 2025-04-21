using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XForms.Data.ModelConfigurations;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        // Relationships
        builder.HasOne(f => f.Creator)
            .WithMany(a => a.Forms)
            .HasForeignKey(f => f.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(f => f.Template)
            .WithMany(t => t.Forms)
            .HasForeignKey(f => f.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // FastQuery
        builder.HasIndex(f => f.TemplateId);
        
        // Optimistic Locking
        builder.Property(f => f.Version)
            .IsConcurrencyToken();
    }
}