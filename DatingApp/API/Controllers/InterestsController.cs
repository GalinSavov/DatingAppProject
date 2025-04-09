using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;
public class InterestsController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult> GetInterests()
    {
        var interests = await unitOfWork.InterestsRepository.GetInterests();
        if (interests == null)
            return BadRequest("Could not find the list of interests in the database");
        return Ok(interests);
    }
    [HttpGet("{username}")]
    public async Task<ActionResult> GetInterestsForUser(string username)
    {
        var interests = await unitOfWork.InterestsRepository.GetInterestsForUser(username);
        if (interests == null) return BadRequest("Could not find interests for user");
        return Ok(interests);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetInterest(int id)
    {
        var interest = await unitOfWork.InterestsRepository.GetInterest(id) ?? throw new Exception("Could not find interest by id!");
        return Ok(interest);
    }
    [HttpPost("{username}/{interestId:int}")]
    public async Task<ActionResult> AddInterestToUser(string username, int interestId)
    {
        var interest = await unitOfWork.InterestsRepository.GetInterest(interestId);
        var currentUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        if (interest == null)
            return BadRequest("Could not find user interest");
        if (currentUser == null)
            return BadRequest("Could not find current user");
        if (currentUser.UserInterests.Count > 4)
        {
            return BadRequest("You have reached the limit for interests in your profile!");
        }
        var userInterest = new AppUserInterest
        {
            UserId = currentUser.Id,
            InterestId = interest.Id
        };
        unitOfWork.InterestsRepository.AddInterestToUser(userInterest);
        if (await unitOfWork.Complete())
            return Ok();
        return BadRequest("Could not add the interest for the user");
    }
    [HttpDelete("{username}/{interestId:int}")]
    public async Task<ActionResult> DeleteInterestFromUser(string username, int interestId)
    {
        var interest = await unitOfWork.InterestsRepository.GetInterest(interestId);
        var currentUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        if (interest == null)
            return BadRequest("Could not find interest");
        if (currentUser == null)
            return BadRequest("Could not find current user");
        var userInterest = await unitOfWork.InterestsRepository.GetAppUserInterest(currentUser.Id, interest.Id);
        if (userInterest == null)
            return BadRequest("Could not find app user interest!");
        unitOfWork.InterestsRepository.DeleteInterestFromUser(userInterest);
        if (await unitOfWork.Complete())
            return Ok();
        return BadRequest("Could not save changes to the user interests database");
    }
}