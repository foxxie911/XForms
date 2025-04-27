using Microsoft.AspNetCore.SignalR;
using XForms.Data;
using XForms.Services;

namespace XForms.Hubs;

public class CommentHub(CommentService commentService) : Hub<ICommentHub>
{
    public async Task SendComment(string message, string creatorId, int templateId)
    {
        var dbComment = await commentService.CreateComment(message, creatorId, templateId);
        if (dbComment is null) return;
        await Clients.All.ReceiveComment(dbComment);
    }

    public async Task LoadComments(int templateId)
    {
        var comments = commentService.GetCommentsByTemplate(templateId);
        await Clients.Caller.LoadComments(comments);
    }

    public async Task DeleteComment(Comment comment)
    {
        var success = await commentService.DeleteComment(comment);
        if (success)
        {
            await Clients.All.RemoveComment(comment);
        }
    }
}