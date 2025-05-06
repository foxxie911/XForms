using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using XForms.Data;

namespace XForms.Services.SignalR;

public class CommentSignalRService : IAsyncDisposable
{
    private readonly HubConnection? _connection;

    public event Action? OnCommentReceived;
    public event Action? OnCommentsLoaded;

    public List<Comment>? Comments { get; private set; }

    public CommentSignalRService(NavigationManager navigationManager)
    {
        var baseUrl = navigationManager.ToAbsoluteUri("/commentHub");
        _connection = new HubConnectionBuilder()
            .WithUrl(baseUrl)
            .WithAutomaticReconnect()
            .Build();
        _connection.On<Comment>("ReceiveComment", (comment) =>
        {
            Comments!.Add(comment);
            OnCommentReceived!.Invoke();
        });
        _connection.On<IEnumerable<Comment>>("LoadComments", (comments) =>
        {
            Comments = comments.OrderByDescending(c => c.CreatedAt).ToList();
            OnCommentsLoaded!.Invoke();
        });
        _connection.On<Comment>("RemoveComment", (comment) =>
        {
            Comments!.Remove(comment);
            OnCommentReceived!.Invoke();
        });
    }

    public async Task StartAsync()
    {
        if (_connection!.State == HubConnectionState.Disconnected)
        {
            await _connection.StartAsync();
        }
    }

    public async Task<bool> SendCommentAsync(string message, string creatorId, int templateId)
    {
        if (_connection!.State == HubConnectionState.Disconnected) return false;
        await _connection.SendAsync("SendComment", message, creatorId, templateId);
        return true;
    }

    public async Task LoadCommentsAsync(int templateId)
    {
        if (_connection!.State == HubConnectionState.Disconnected) return;
        await _connection.SendAsync("LoadComments", templateId);
    }

    public async Task<bool> DeleteCommentAsync(Comment comment)
    {
        if (_connection!.State == HubConnectionState.Disconnected) return false;
        await _connection.SendAsync("DeleteComment", comment);
        return true;
    }

    public bool IsConnected => _connection!.State == HubConnectionState.Connected;

    public async ValueTask DisposeAsync()
    {
        if (_connection is null) return;
        await _connection.DisposeAsync();
    }
}