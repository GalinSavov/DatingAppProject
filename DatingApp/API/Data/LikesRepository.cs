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
    }

    public void DeleteLike(UserLike like)
    {
        dataContext.Likes.Remove(like);
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
    public async Task<bool> HasMutualLikeWithAnotherUser(string sourceUsername, string targetUsername)
    {
        var sourceUser = await dataContext.Users.FirstOrDefaultAsync(x => x.UserName == sourceUsername) ?? throw new Exception("User not found in repository");
        var targetUser = await dataContext.Users.FirstOrDefaultAsync(x => x.UserName == targetUsername) ?? throw new Exception("User not found in repository");
        var currentUserLikeIds = await GetCurrentUserLikeIds(sourceUser.Id);
        return await dataContext.Likes.Where(like => like.TargetUserId == sourceUser.Id
        && currentUserLikeIds.Contains(targetUser.Id)
        && like.SourceUserId == targetUser.Id).AnyAsync();
    }

}