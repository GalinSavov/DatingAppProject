using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ErrorController(DataContext dataContext) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth() //401
    {
        return "secret text here";
    }
    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound() //404
    {
        var user = dataContext.Users.Find(-1);
        if (user == null) return NotFound();
        else
            return user;
    }
    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError() //500
    {
        var user = dataContext.Users.Find(-1) ?? throw new Exception("A bad thing has happened");
        return user;
    }
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest() //400
    {
        return BadRequest("This was not a good request");
    }
}