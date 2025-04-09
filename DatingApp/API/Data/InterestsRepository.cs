using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace API.Data;
public class InterestsRepository(DataContext dataContext) : IInterestsRepository
{
    public void AddInterestToUser(AppUserInterest userInterest)
    {
        if (dataContext.UserInterests.Any(i => i.InterestId == userInterest.InterestId && i.UserId == userInterest.UserId))
        {
            return;
        }
        dataContext.UserInterests.Add(userInterest);
    }
    public void DeleteInterestFromUser(AppUserInterest userInterest)
    {
        if (!dataContext.UserInterests.Any(i => i.InterestId == userInterest.InterestId && i.UserId == userInterest.UserId))
        {
            return;
        }
        dataContext.UserInterests.Remove(userInterest);
    }
    public async Task<AppUserInterest> GetAppUserInterest(int userId, int interestId)
    {
        var appUserInterest = await dataContext.UserInterests.FirstOrDefaultAsync(x => x.UserId == userId && x.InterestId == interestId) ?? throw new Exception("Could not find app user interest");
        return appUserInterest;
    }
    public async Task<Interest> GetInterest(int id)
    {
        var interest = await dataContext.Interests.FindAsync(id) ?? throw new Exception("Interest could not be found by id!");
        return interest;
    }
    public async Task<IEnumerable<Interest>> GetInterests()
    {
        return await dataContext.Interests.Where(x => x.Id != 0).ToListAsync();
    }
    public async Task<IEnumerable<Interest>> GetInterestsForUser(string username)
    {
        return await dataContext.UserInterests.Where(x => x.User.UserName == username).Select(x => x.Interest).ToListAsync();
    }
}