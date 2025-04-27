using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XForms.Data.ModelConfigurations;

namespace XForms.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Template> Templates { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Model Configurations
        builder.ApplyConfiguration(new TemplateConfiguration());
        builder.ApplyConfiguration(new QuestionConfiguration());
        builder.ApplyConfiguration(new FormConfiguration());
        builder.ApplyConfiguration(new AnswerConfiguration());
        builder.ApplyConfiguration(new CommentConfiguration());
        builder.ApplyConfiguration(new LikeConfiguration());
    }
}
