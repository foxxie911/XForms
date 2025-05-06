using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;

namespace XForms.Components.Template;

public partial class CommentBox : ComponentBase
{
    // Parameter
    [Parameter] public int TemplateId { get; set; }

    // Dependency Injection
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }

    // Class Variable
    private ApplicationUser? _currentUser;
    private Comment? _selectedComment;
    private MudMenu? _commentControlPanel;
    private string? _commentMessage;
    private IEnumerable<Comment>? _comments;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var authState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
        _currentUser = await UserManager!.GetUserAsync(authState.User);
        await CommentSignalRService!.StartAsync();
        CommentSignalRService!.OnCommentReceived += StateHasChanged;
        CommentSignalRService!.OnCommentsLoaded += StateHasChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await CommentSignalRService!.LoadCommentsAsync(TemplateId);
        _comments = CommentSignalRService!.Comments;
    }

    private void OpenControlPanel(MouseEventArgs args, Comment comment)
    {
        _selectedComment = comment;
        _commentControlPanel!.OpenMenuAsync(args);
    }

    private async Task DeleteComment()
    {
        var success = await CommentSignalRService!.DeleteCommentAsync(_selectedComment!);
        _selectedComment = null;
        if (success)
            Snackbar!.Add("Comment deleted", Severity.Success);
        if (!success)
            Snackbar!.Add("Comment could not be deleted", Severity.Error);
    }

    private async Task PostComment()
    {
        if (string.IsNullOrWhiteSpace(_commentMessage)) return;
        var success = await CommentSignalRService!.SendCommentAsync(_commentMessage, _currentUser!.Id, TemplateId);
        if (success)
            Snackbar!.Add("Comment sent", Severity.Success);
        if (!success)
            Snackbar!.Add("Comment could not be sent. Something went wrong.", Severity.Error);
    }
}