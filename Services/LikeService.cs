using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class LikeService(ApplicationDbContext context)
{
    public async Task LikeOrUnlikeTemplate(string userId, int templateId)
    {
        try
        {
            var existingLike = await context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.TemplateId == templateId);

            if (existingLike is null)
            {
                await context.Likes.AddAsync(new Like()
                {
                    UserId = userId,
                    TemplateId = templateId
                });
            }

            if (existingLike is not null)
            {
                context.Likes.Remove(existingLike);
            }

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
    }

    public async Task<int> CountLikeByTemplateIdAsync(int templateId)
    {
        try
        {
            var likeCount = await context.Likes
                .Where(l => l.TemplateId == templateId)
                .AsNoTracking()
                .CountAsync();
            return likeCount;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return -1;
    }

    public async Task<bool> IsLikedAsync(string userId, int templateId)
    {
        try
        {
            var like = await context.Likes
                .Where(l => l.TemplateId == templateId && l.UserId == userId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (like != null) return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }
}