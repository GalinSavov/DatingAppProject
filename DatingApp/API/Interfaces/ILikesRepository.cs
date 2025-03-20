using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);
    Task<PagedList<MemberDTO>> GetUserLikes(LikesParams likesParams);
    Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
    Task<bool> HasMutualLikeWithAnotherUser(string sourceUsername, string targetUsername);
    void DeleteLike(UserLike like);
    void AddLike(UserLike like);
}