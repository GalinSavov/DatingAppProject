using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetId();
        if (sourceUserId == targetUserId) return BadRequest("Can not like your own profile!");

        var existingLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, targetUserId);
        if (existingLike != null)
        {
            unitOfWork.LikesRepository.DeleteLike(existingLike);
        }
        else
        {
            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            unitOfWork.LikesRepository.AddLike(like);
        }

        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("Failed to update the toggle of a like");
    }

    [HttpGet("get-user-like")]
    public async Task<UserLike?> GetUserLike(int sourceId, int targetId)
    {
        var sourceUser = unitOfWork.UserRepository.GetUserByIdAsync(sourceId);
        var targetUser = unitOfWork.UserRepository.GetUserByIdAsync(targetId);
        if (sourceUser == null || targetUser == null || sourceId != targetId) return new UserLike();

        var userLike = unitOfWork.LikesRepository.GetUserLike(sourceUser.Id, targetUser.Id);
        return await userLike;
    }
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        var currentUserID = User.GetId();
        var currentUserLikeIds = unitOfWork.LikesRepository.GetCurrentUserLikeIds(currentUserID);
        return Ok(await currentUserLikeIds);
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetId();
        var users = await unitOfWork.LikesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users);
        return Ok(users);
    }
}