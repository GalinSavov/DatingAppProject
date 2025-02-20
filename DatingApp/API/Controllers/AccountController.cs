using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext dataContext, IMapper mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO, ITokenService tokenService)
    {
        if (await UserExists(registerDTO.Username))
            return BadRequest("Username is already taken!");
        using var hmac = new HMACSHA512();
        var user = mapper.Map<AppUser>(registerDTO);
        user.UserName = registerDTO.Username.ToLower();
        dataContext.Users.Add(user);
        await dataContext.SaveChangesAsync();
        return new UserDTO
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender,
        };
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO, ITokenService tokenService)
    {
        var appUser = await dataContext.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());

        if (appUser == null)
            return Unauthorized("Invalid username!");

        return new UserDTO
        {
            Username = appUser.UserName,
            Token = tokenService.CreateToken(appUser),
            KnownAs = appUser.KnownAs,
            Gender = appUser.Gender,
            PhotoUrl = appUser.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
        };
    }
    private async Task<bool> UserExists(string username)
    {
        return await dataContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
