using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XForms.Data.ModelConfigurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
   public void Configure(EntityTypeBuilder<Tag> builder)
   {
      // No relationship here
      // Fast query
      builder.HasIndex(t => t.Name);
   }
}