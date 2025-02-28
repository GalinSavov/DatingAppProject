using API.Interfaces;

namespace API.Data;

public class UnitOfWork(DataContext dataContext, IUserRepository userRepository,
IMessagesRepository messagesRepository, ILikesRepository likesRepository) : IUnitOfWork
{
    public IUserRepository UserRepository => userRepository;

    public IMessagesRepository MessagesRepository => messagesRepository;

    public ILikesRepository LikesRepository => likesRepository;

    public async Task<bool> Complete()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return dataContext.ChangeTracker.HasChanges();
    }
}