namespace XForms.Services.Interface;

public interface ILikeService
{
    Task LikeOrUnlikeTemplate(string userId, int templateId);
    Task<int> CountLikeByTemplateIdAsync(int templateId);
    Task<bool> IsLikedAsync(string userId, int templateId);
}
