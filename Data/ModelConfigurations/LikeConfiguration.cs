using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XForms.Data.ModelConfigurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        // Relations 
        builder.HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Template)
            .WithMany(t => t.Likes)
            .HasForeignKey(l => l.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Fast query
        builder.HasIndex(l => l.UserId);
    }
}