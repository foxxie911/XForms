
using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class CommentService(ApplicationDbContext context)
{
    public IEnumerable<Comment> GetCommentsByTemplate(int templateId)
    {
        try
        {
            var answer = context.Comments
                .Where(c => c.TemplateId == templateId)
                .Include(c => c.Creator);
            return answer;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }

    public async Task<Comment> CreateComment(string message, string creatorId, int templateId)
    {
        try { var dbComment = context.Comments.Add(new Comment() {
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

    public async Task<bool> DeleteComment(Comment comment)
    {
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