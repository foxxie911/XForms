using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XForms.Data.ModelConfigurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
   public void Configure(EntityTypeBuilder<Template> builder)
   {
       // Relationships
        builder.HasOne(t => t.Creator)
            .WithMany(u => u.Templates)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Tags)
            .WithMany(t => t.Templates)
            .UsingEntity<TemplateTag>();

        // Fast Query
        builder.HasIndex(t => new{ t.Title, t.CreatorId});
        
        // Concurrency or Optimistic Locking
        builder.Property(t => t.Version)
            .IsConcurrencyToken();
   }
}