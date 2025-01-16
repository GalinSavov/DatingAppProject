using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext dataContext):BaseApiController 
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO,ITokenService tokenService)
    {

        if(await UserExists(registerDTO.Username))
           return BadRequest("Username is already taken!");

        using var hmac = new HMACSHA512();
        AppUser user = new AppUser
        {
            UserName = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
        dataContext.Users.Add(user);
        await dataContext.SaveChangesAsync();

        return new UserDTO
        {
            Username = registerDTO.Username,
            Token = tokenService.CreateToken(user)
        };
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO,ITokenService tokenService)
    {
       var appUser = await dataContext.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());

       if(appUser == null)
          return Unauthorized("Invalid username!");

       using var hmac = new HMACSHA512(appUser.PasswordSalt);
       var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));   

       for (int i = 0; i < computedHash.Length; i++)
       {
            if(computedHash[i] != appUser.PasswordHash[i])
               return Unauthorized("Invalid password!");
       }
        return new UserDTO
        {
            Username = appUser.UserName,
            Token = tokenService.CreateToken(appUser)
        };
    }
    private async Task<bool> UserExists(string username)
    {
        return await dataContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
