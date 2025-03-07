namespace API.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IMessagesRepository MessagesRepository { get; }
    ILikesRepository LikesRepository { get; }
    IPhotosRepository PhotosRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}