using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HackStack___Gemini.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetUserProfile()
        {
            var username = User.Identity?.Name;
            return Ok(new { message = $"Welcome, {username}!" });
        }
    }
}
