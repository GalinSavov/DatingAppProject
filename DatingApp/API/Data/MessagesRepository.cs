using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;

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
            "Inbox" => query.Where(x => x.RecipientUsername == messageParams.Username),
            "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username),
            _ => query.Where(x => x.RecipientUsername == messageParams.Username && x.DateRead == null)
        };
        var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.ItemsPerPage, messageParams.CurrentPageNumber);
    }

    public Task<IEnumerable<MessageDTO>> GetMessageThread(string senderUsername, string recipientUsername)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }
}