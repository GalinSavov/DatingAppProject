using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseApiController
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
    // [Authorize(Policy = "ModeratePhotoRole")]
    // [HttpGet("photos-to-moderate")]
    // public Task<ActionResult> GetPhotosForModeration()
    // {
    //     return BadRequest()
    // }
}