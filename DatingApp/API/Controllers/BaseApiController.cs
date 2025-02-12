namespace API.Controllers;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;
[ServiceFilter(typeof(LogUserActivity))]
[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{

}



