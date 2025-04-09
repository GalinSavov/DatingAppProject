using API.Entities;

namespace API.Interfaces;

public interface IInterestsRepository
{
    Task<Interest> GetInterest(int id);
    Task<AppUserInterest> GetAppUserInterest(int userId, int interestId);
    Task<IEnumerable<Interest>> GetInterests();
    Task<IEnumerable<Interest>> GetInterestsForUser(string username);
    void AddInterestToUser(AppUserInterest userInterest);
    void DeleteInterestFromUser(AppUserInterest userInterest);
}