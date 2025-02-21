using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, IMapper mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO, ITokenService tokenService)
    {
        if (await UserExists(registerDTO.Username))
            return BadRequest("Username is already taken!");

        var user = mapper.Map<AppUser>(registerDTO);
        user.UserName = registerDTO.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDTO.Password); //saves the user in the database
        if (!result.Succeeded) return BadRequest(result.Errors);

        return new UserDTO
        {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender,
        };
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO, ITokenService tokenService)
    {
        var appUser = await userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.NormalizedUserName == loginDTO.Username.ToUpper());

        if (appUser == null || appUser.UserName == null)
            return Unauthorized("Invalid username!");

        var result = await userManager.CheckPasswordAsync(appUser, loginDTO.Password);
        if (!result)
            return Unauthorized("Invalid password!");

        return new UserDTO
        {
            Username = appUser.UserName,
            Token = await tokenService.CreateToken(appUser),
            KnownAs = appUser.KnownAs,
            Gender = appUser.Gender,
            PhotoUrl = appUser.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
        };
    }
    private async Task<bool> UserExists(string username)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
    }
}
