using System.Diagnostics;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]
public class UsersController(IMapper mapper, IPhotoService photoService, IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
    {
        userParams.CurrentUsername = User.GetUsername();
        var users = await unitOfWork.UserRepository.GetAllMembersAsync(userParams);
        Response.AddPaginationHeader(users);
        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        MemberDTO? user;
        if (username == User.GetUsername())
        {
            user = await unitOfWork.UserRepository.GetMemberByUsernameAsync(username, true);
        }
        else
        {
            user = await unitOfWork.UserRepository.GetMemberByUsernameAsync(username, false);
        }
        if (user == null) return NotFound();
        return user;
    }
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");
        mapper.Map(memberUpdateDTO, user);
        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to update user");
    }
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> UploadPhoto(IFormFile file)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Cannot find user");
        var result = await photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);
        var photo = new Photo
        {
            Url = result.Url.AbsoluteUri,
            PublicId = result.PublicId,
            IsApproved = false,
        };
        //if (user.Photos.Count == 0) photo.IsMain = true;
        user.Photos.Add(photo);
        if (await unitOfWork.Complete()) return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDTO>(photo));
        return BadRequest("Problem adding photo!");
    }
    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Cannot find user");
        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null || photo.IsMain) return BadRequest("Can not use this as main photo");
        var currentMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain == true);
        if (currentMainPhoto == null) return BadRequest("Current main photo error");
        currentMainPhoto.IsMain = false;
        photo.IsMain = true;
        if (await unitOfWork.Complete()) return NoContent();
        return BadRequest("Failed to update the main photo of the user");
    }
    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Cannot find user");
        var photo = await unitOfWork.PhotosRepository.GetPhotoById(photoId);
        if (photo == null || photo.IsMain) return BadRequest("Can not delete that photo");
        var photoPublicId = photo.PublicId;
        if (photoPublicId == null) return BadRequest("Public id of photo is null");
        var result = await photoService.DeletePhotoAsync(photoPublicId);
        if (result.Error != null) return BadRequest(result.Error.Message);
        user.Photos.Remove(photo);
        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("Problem deleting photo!");
    }

}