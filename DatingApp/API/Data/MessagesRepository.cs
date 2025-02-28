using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;


public class MessagesRepository(DataContext dataContext, IMapper mapper) : IMessagesRepository
{
    public void Add(Message message)
    {
        dataContext.Messages.Add(message);
    }
    public void Delete(Message message)
    {
        dataContext.Messages.Remove(message);
    }
    public async Task<Message?> GetMessageAsync(int id)
    {
        return await dataContext.Messages.FindAsync(id);
    }
    public async Task<PagedList<MessageDTO>> GetMessagesForUserAsync(MessageParams messageParams)
    {
        var query = dataContext.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();
        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.RecipientUsername == messageParams.Username && x.RecipientDeleted == false),
            "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username && x.SenderDeleted == false),
            _ => query.Where(x => x.RecipientUsername == messageParams.Username && x.DateRead == null && x.RecipientDeleted == false),
        };
        var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.ItemsPerPage, messageParams.CurrentPageNumber);
    }
    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserUsername, string recipientUsername)
    {
        var messages = await dataContext.Messages.
        Where(x => x.SenderUsername == currentUserUsername && x.RecipientUsername == recipientUsername && !x.SenderDeleted ||
        x.RecipientUsername == currentUserUsername && x.SenderUsername == recipientUsername && !x.RecipientDeleted).
        OrderBy(x => x.MessageSent).
        ProjectTo<MessageDTO>(mapper.ConfigurationProvider).
        ToListAsync();

        var unreadMessages = messages.Where(x => x.DateRead == null && x.RecipientUsername == currentUserUsername).ToList();
        if (unreadMessages.Count > 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
        }
        return messages;
    }
    public void AddMessageGroup(Group group)
    {
        dataContext.Groups.Add(group);
    }
    public void RemoveConnection(Connection connection)
    {
        dataContext.Connections.Remove(connection);
    }
    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await dataContext.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<Connection?> GetMessageConnection(string connectionId)
    {
        return await dataContext.Connections.FindAsync(connectionId);
    }
    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await dataContext.Groups.Include(x => x.Connections).Where(x => x.Connections.Any(c => c.ConnectionId == connectionId)).FirstOrDefaultAsync();
    }
}