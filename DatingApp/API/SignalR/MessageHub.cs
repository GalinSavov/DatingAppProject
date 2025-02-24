using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessagesRepository messagesRepository) : Hub
{
    public async override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];
        if (Context.User?.GetUsername() == null || string.IsNullOrEmpty(otherUser))
            throw new HubException("Could not join group chat");
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var messages = messagesRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
    private string GetGroupName(string caller, string? other)
    {
        var compareString = string.CompareOrdinal(caller, other);
        if (compareString == -1)
            return caller + "-" + other;
        else
            return other + "-" + caller;
    }
}