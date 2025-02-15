using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepository likesRepository, IUserRepository userRepository) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetId();
        if (sourceUserId == targetUserId) return BadRequest("Can not like your own profile!");

        var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);
        if (existingLike != null)
        {
            likesRepository.DeleteLike(existingLike);
        }
        else
        {
            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            likesRepository.AddLike(like);
        }

        if (await likesRepository.SaveChanges()) return Ok();
        return BadRequest("Failed to update the toggle of a like");
    }

    [HttpGet("get-user-like")]
    public async Task<UserLike?> GetUserLike(int sourceId, int targetId)
    {
        var sourceUser = userRepository.GetUserByIdAsync(sourceId);
        var targetUser = userRepository.GetUserByIdAsync(targetId);
        if (sourceUser == null || targetUser == null || sourceId != targetId) return new UserLike();

        var userLike = likesRepository.GetUserLike(sourceUser.Id, targetUser.Id);
        return await userLike;
    }
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        var currentUserID = User.GetId();
        var currentUserLikeIds = likesRepository.GetCurrentUserLikeIds(currentUserID);
        return Ok(await currentUserLikeIds);
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetId();
        var users = await likesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users);
        return Ok(users);
    }
}