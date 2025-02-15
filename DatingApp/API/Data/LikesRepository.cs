using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

    public async Task<PagedList<MemberDTO>> GetUserLikes(LikesParams likesParams)
    {
        var likes = dataContext.Likes.AsQueryable();
        IQueryable<MemberDTO> query;

        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes.Where(x => x.SourceUserId == likesParams.UserId).
                Select(x => x.TargetUser).
                ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes.Where(x => x.TargetUserId == likesParams.UserId).
                Select(x => x.SourceUser).
                ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
            default:
                var currentUserLikeids = await GetCurrentUserLikeIds(likesParams.UserId);
                query = likes.Where(like => like.TargetUserId == likesParams.UserId && currentUserLikeids.Contains(like.SourceUserId)).
                Select(like => like.SourceUser).
                ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
        }
        return await PagedList<MemberDTO>.CreateAsync(query, likesParams.ItemsPerPage, likesParams.CurrentPageNumber);
    }

    public async Task<bool> SaveChanges()
    {
        if (await dataContext.SaveChangesAsync() > 0) return true;
        return false;
    }
}