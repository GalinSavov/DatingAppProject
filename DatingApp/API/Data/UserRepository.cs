using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext dataContext, IMapper mapper) : IUserRepository
{
    public async Task<PagedList<MemberDTO>> GetAllMembersAsync(UserParams userParams)
    {
        var query = dataContext.Users.AsQueryable();
        query = query.Where(x => x.UserName != userParams.CurrentUsername);
        if (userParams.Gender != null)
        {
            query = query.Where(x => x.Gender == userParams.Gender);
        }
        var oldestDateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var youngestDateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
        query = query.Where(x => x.DateOfBirth >= oldestDateOfBirth && x.DateOfBirth <= youngestDateOfBirth);
        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
        };
        if (userParams.Interests != null)
        {
            query = query.Where(user => user.UserInterests.Any(x => userParams.Interests.Contains(x.Interest.Name)))
            .OrderByDescending(user => user.UserInterests.Count(z => userParams.Interests.Contains(z.Interest.Name)));
        }
        

        return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider), userParams.ItemsPerPage, userParams.CurrentPageNumber);
    }
    public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
    {
        return await dataContext.Users.Include(x => x.Photos).ToListAsync();
    }
    public async Task<MemberDTO?> GetMemberByUsernameAsync(string username, bool isCurrentUser)
    {
        return isCurrentUser ?
        await dataContext.Users.Where(x => x.UserName == username).IgnoreQueryFilters().
        ProjectTo<MemberDTO>(mapper.ConfigurationProvider).
        SingleOrDefaultAsync() :
        await dataContext.Users.Where(x => x.UserName == username).
        ProjectTo<MemberDTO>(mapper.ConfigurationProvider).
        SingleOrDefaultAsync();
    }
    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await dataContext.Users.FindAsync(id);
    }
    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await dataContext.Users.Include(x => x.Photos).Include(z => z.UserInterests).SingleOrDefaultAsync(x => x.UserName == username);
    }
    public void Update(AppUser appUser)
    {
        dataContext.Entry(appUser).State = EntityState.Modified;
    }
    public async Task<AppUser?> GetUserByPhotoId(int photoId)
    {
        return await dataContext.Users.Include(x => x.Photos).IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Photos.Any(x => x.Id == photoId));
    }
}