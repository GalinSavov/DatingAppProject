using API.Interfaces;

namespace API.Data;

public class UnitOfWork(DataContext dataContext, IUserRepository userRepository,
IMessagesRepository messagesRepository, ILikesRepository likesRepository, IPhotosRepository photosRepository, IInterestsRepository interestsRepository) : IUnitOfWork
{
    public IUserRepository UserRepository => userRepository;
    public IMessagesRepository MessagesRepository => messagesRepository;
    public ILikesRepository LikesRepository => likesRepository;
    public IPhotosRepository PhotosRepository => photosRepository;
    public IInterestsRepository InterestsRepository => interestsRepository;
    public async Task<bool> Complete()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }
    public bool HasChanges()
    {
        return dataContext.ChangeTracker.HasChanges();
    }
}