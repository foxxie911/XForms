using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XForms.Data.ModelConfigurations;

namespace XForms.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Template> Templates { get; set; }
    public DbSet<Question> Questions { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Model Configurations
        builder.ApplyConfiguration(new TemplateConfiguration());
        builder.ApplyConfiguration(new QuestionConfiguration());
    }
}
