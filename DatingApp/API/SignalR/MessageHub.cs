using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessagesRepository messagesRepository, IUserRepository userRepository,
IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
{
    public async override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];
        if (Context.User?.GetUsername() == null || string.IsNullOrEmpty(otherUser))
            throw new HubException("Could not join group chat");
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToMessageGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        var messages = messagesRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }
    private string GetGroupName(string caller, string? other)
    {
        var compareString = string.CompareOrdinal(caller, other) < 0;
        return compareString ? $"{caller}-{other}" : $"{other}-{caller}";
    }
    public async Task SendMessage(CreateMessageDTO createMessageDTO)
    {
        var username = Context?.User?.GetUsername() ?? throw new Exception("Could not get user");
        if (username == createMessageDTO.RecipientUsername.ToLower()) throw new HubException("You cannot message yourself");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

        if (sender == null || recipient == null ||
        sender.UserName == null || recipient.UserName == null) throw new HubException("Either the sender or recipient does not exist");

        var newMessage = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content,
        };
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await messagesRepository.GetMessageGroup(groupName);
        if (group != null && group.Connections.Any(x => x.Username == recipient.UserName))
        {
            newMessage.DateRead = DateTime.UtcNow;
        }
        else
        {
            var recipientConnections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (recipientConnections != null && recipientConnections?.Count != null)
            {
                await presenceHub.Clients.Clients(recipientConnections).SendAsync("NewMessageReceived!", new
                {
                    username = sender.UserName,
                    knownAs = sender.KnownAs,
                });
            }
        }
        messagesRepository.Add(newMessage);
        if (await messagesRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(newMessage));
        }
    }
    private async Task<Group> AddToMessageGroup(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Could not find user");
        var group = await messagesRepository.GetMessageGroup(groupName);
        var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };
        if (group == null)
        {
            group = new Group { Name = groupName };
            messagesRepository.AddMessageGroup(group);
        }
        group.Connections.Add(connection);
        if (await messagesRepository.SaveAllAsync()) return group;
        throw new HubException("Failed to join group");
    }
    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await messagesRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (connection == null)
        {
            throw new HubException("Connection to group not found");
        }
        messagesRepository.RemoveConnection(connection);
        if (await messagesRepository.SaveAllAsync() && group != null)
        {
            return group;
        }
        throw new Exception("Could not remove the connection from the message group");
    }
}