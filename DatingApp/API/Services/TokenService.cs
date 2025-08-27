using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration configuration, UserManager<AppUser> userManager) : ITokenService
{
    public async Task<string> CreateToken(AppUser user)
    {
        var tokenKey = configuration["TokenKey"] ?? throw new Exception("Can't access tokenKey from appsettings!"); //check for TokenKey in the env variables in appsettings.json
        if (tokenKey.Length < 64) throw new Exception("Token key length needs to be longer!");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)); //encode the env variable TokenKey from appsettings.json using UTF-8 format into a key

        if (user.UserName == null) throw new Exception("Username not found!");
        var claims = new List<Claim> //information about the user that will be included in the token, when the server decodes the token payload, it can identify the user with this information
        {
            new(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new(ClaimTypes.Name,user.UserName)
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512); //signs the key with Hmac512 algorithm

        var tokenDescriptor = new SecurityTokenDescriptor //adds properties to the token
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}