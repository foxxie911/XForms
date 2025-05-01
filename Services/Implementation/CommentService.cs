using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Services.Implementation;

public class CommentService(IDbContextFactory<ApplicationDbContext> contextFactory) : ICommentService
{
    public async Task<List<Comment>> GetCommentsByTemplateAsync(int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var answer = await context.Comments
                .Where(c => c.TemplateId == templateId)
                .Include(c => c.Creator)
                .ToListAsync();
            return answer;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }

    public async Task<Comment> CreateCommentAsync(string message, string creatorId, int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try { var dbComment = await context.Comments.AddAsync(new Comment() {
                Message= message,
                CreatorId = creatorId,
                TemplateId = templateId,
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            return dbComment.Entity;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
        
        return null!;
    }

    public async Task<bool> DeleteCommentAsync(Comment comment)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }
}