using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class LikeService(ApplicationDbContext context)
{
    public void LikeOrUnlikeTemplate(string userId, int templateId)
    {
        try
        {
            var existingLike = context.Likes
                .FirstOrDefault(l => l.UserId == userId && l.TemplateId == templateId);

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

            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
    }

    public int CountLikeByTemplateId(int templateId)
    {
        try
        {
            var likeCount = context.Likes
                .Where(l => l.TemplateId == templateId)
                .AsNoTracking()
                .Count();
            return likeCount;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return -1;
    }

    public bool IsLikedAsync(string userId, int templateId)
    {
        try
        {
            var like = context.Likes
                .Where(l => l.TemplateId == templateId && l.UserId == userId)
                .AsNoTracking()
                .FirstOrDefault();
            if (like != null) return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }
}