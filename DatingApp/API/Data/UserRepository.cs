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

        return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider), userParams.ItemsPerPage, userParams.CurrentPageNumber);
    }

    public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
    {
        return await dataContext.Users.Include(x => x.Photos).ToListAsync();
    }

    public async Task<MemberDTO?> GetMemberByUsernameAsync(string username)
    {
        return await dataContext.Users.Where(x => x.UserName == username).
        ProjectTo<MemberDTO>(mapper.ConfigurationProvider).
        SingleOrDefaultAsync();
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await dataContext.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await dataContext.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }

    public void Update(AppUser appUser)
    {
        dataContext.Entry(appUser).State = EntityState.Modified;
    }
}