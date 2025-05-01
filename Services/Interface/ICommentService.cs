using XForms.Data;

namespace XForms.Services.Interface;

public interface ICommentService
{
    Task<List<Comment>> GetCommentsByTemplateAsync(int templateId);
    Task<Comment> CreateCommentAsync(string message, string creatorId, int templateId);
    Task<bool> DeleteCommentAsync(Comment comment);
}