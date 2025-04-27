using XForms.Data;

namespace XForms.Hubs;

public interface ICommentHub
{
    Task ReceiveComment(Comment comment);
    Task LoadComments(IEnumerable<Comment> comments);
    Task RemoveComment(Comment comment);

}