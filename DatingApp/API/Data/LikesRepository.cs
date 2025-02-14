using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext dataContext, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        dataContext.Likes.Add(like);
        //await SaveChanges();
    }

    public void DeleteLike(UserLike like)
    {
        dataContext.Likes.Remove(like);
        //await SaveChanges();
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await dataContext.Likes.Where(x => x.SourceUserId == currentUserId).Select(x => x.TargetUserId).ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        var userLike = dataContext.Likes.FindAsync(sourceUserId, targetUserId);
        return await userLike;
    }

    public async Task<IEnumerable<MemberDTO>> GetUserLikes(string predicate, int userId)
    {
        var likes = dataContext.Likes.AsQueryable();

        switch (predicate)
        {
            case "liked":
                return await likes.Where(x => x.SourceUserId == userId).
                Select(x => x.TargetUser).
                ProjectTo<MemberDTO>(mapper.ConfigurationProvider).
                ToListAsync();
            case "likedBy":
                return await likes.Where(x => x.TargetUserId == userId).
                Select(x => x.SourceUser).
                ProjectTo<MemberDTO>(mapper.ConfigurationProvider).
                ToListAsync();
            default:
                var currentUserLikeids = await GetCurrentUserLikeIds(userId);
                return await likes.Where(like => like.TargetUserId == userId && currentUserLikeids.Contains(like.SourceUserId)).
                Select(like => like.SourceUser).
                ProjectTo<MemberDTO>(mapper.ConfigurationProvider).
                ToListAsync();
        }
    }

    public async Task<bool> SaveChanges()
    {
        if (await dataContext.SaveChangesAsync() > 0) return true;
        return false;
    }
}