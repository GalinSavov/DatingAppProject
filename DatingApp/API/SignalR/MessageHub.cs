using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessagesRepository messagesRepository, IUserRepository userRepository, IMapper mapper) : Hub
{
    public async override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];
        if (Context.User?.GetUsername() == null || string.IsNullOrEmpty(otherUser))
            throw new HubException("Could not join group chat");
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToMessageGroup(groupName);
        var messages = messagesRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await RemoveFromMessageGroup();
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
        messagesRepository.Add(newMessage);
        if (await messagesRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(newMessage));
        }
    }
    private async Task<bool> AddToMessageGroup(string groupName)
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
        return await messagesRepository.SaveAllAsync();
    }
    private async Task<bool> RemoveFromMessageGroup()
    {
        var connection = await messagesRepository.GetMessageConnection(Context.ConnectionId);
        if (connection == null)
        {
            throw new Exception("GROUP NOT FOUND");
        }
        messagesRepository.RemoveConnection(connection);
        return await messagesRepository.SaveAllAsync();
    }
}