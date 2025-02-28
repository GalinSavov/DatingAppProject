using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessagesRepository
{
    void Add(Message message);
    void Delete(Message message);
    Task<Message?> GetMessageAsync(int id);
    Task<PagedList<MessageDTO>> GetMessagesForUserAsync(MessageParams messageParams);
    Task<IEnumerable<MessageDTO>> GetMessageThread(string senderUsername, string recipientUsername);
    Task<bool> SaveAllAsync();
    void AddMessageGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Group?> GetMessageGroup(string groupName);
    Task<Connection?> GetMessageConnection(string connectionId);
    Task<Group?> GetGroupForConnection(string connectionId);
}