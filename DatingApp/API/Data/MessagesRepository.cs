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
        Include(x => x.Sender).ThenInclude(x => x.Photos).
        Include(x => x.Recipient).ThenInclude(x => x.Photos).
        Where(x => x.SenderUsername == currentUserUsername && x.RecipientUsername == recipientUsername && !x.SenderDeleted ||
        x.RecipientUsername == currentUserUsername && x.SenderUsername == recipientUsername && !x.RecipientDeleted).
        OrderBy(x => x.MessageSent).
        ToListAsync();

        var unreadMessages = messages.Where(x => x.DateRead == null && x.RecipientUsername == currentUserUsername).ToList();
        if (unreadMessages.Count > 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            await dataContext.SaveChangesAsync();
        }
        return mapper.Map<IEnumerable<MessageDTO>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }
}