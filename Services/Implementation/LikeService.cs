using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Services.Implementation;

public class LikeService(IDbContextFactory<ApplicationDbContext> contextFactory) : ILikeService
{
    public async Task LikeOrUnlikeTemplate(string userId, int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var existingLike = await context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.TemplateId == templateId);

            if (existingLike is null)
            {
                context.Likes.Add(new Like()
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
            Console.WriteLine($@"{e.Message}");
        }
    }

    public async Task<int> CountLikeByTemplateIdAsync(int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
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
            Console.WriteLine($@"{e.Message}");
        }

        return -1;
    }

    public async Task<bool> IsLikedAsync(string userId, int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
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
            Console.WriteLine($@"{e.Message}");
        }

        return false;
    }
}