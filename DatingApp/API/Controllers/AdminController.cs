using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers;
public class AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users.OrderBy(x => x.UserName).Select(x => new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(x => x.Role.Name).ToList()
        }).ToListAsync();
        if (users.Count == 0 || users == null) return BadRequest("No users found!");
        return Ok(users);
    }
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("No role has been specified!");
        var selectedRoles = roles.Split(",").ToArray();
        var user = await userManager.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
        if (user == null) return BadRequest("Could not find user");
        var userRoles = await userManager.GetRolesAsync(user);
        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest(result.Errors);
        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok(await userManager.GetRolesAsync(user));
    }
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForApproval()
    {
        var photos = await unitOfWork.PhotosRepository.GetUnapprovedPhotos();
        if (photos == null) return BadRequest("Could not retrieve unapproved photos!");
        return Ok(photos);
    }
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId:int}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await unitOfWork.PhotosRepository.GetPhotoById(photoId);
        if (photo == null) return BadRequest("Could not find the photo!");
        if (photo.IsApproved == true) return BadRequest("Photo was already approved!");
        var user = await unitOfWork.UserRepository.GetUserByPhotoId(photo.Id);
        if (user == null) return BadRequest("Could not find user by photo id!");
        if (!user.Photos.Any(x => x.IsMain == true))
        {
            photo.IsMain = true;
        }
        photo.IsApproved = true;
        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("Could not approve photo!");
    }
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpDelete("reject-photo/{photoId:int}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await unitOfWork.PhotosRepository.GetPhotoById(photoId);
        if (photo == null) return BadRequest("Could not find the photo!");
        if (photo.IsApproved != false) return BadRequest("Can not reject an already approved photo!");
        unitOfWork.PhotosRepository.RemovePhoto(photo);
        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("Could not delete photo!");
    }
}