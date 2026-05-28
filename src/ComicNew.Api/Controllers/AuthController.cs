using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Claims;
 
namespace ComicNew.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var userId = User.FindFirstValue("sub");
            var email = User.FindFirstValue("email");
            var role = User.FindFirstValue("role");

            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new
            {
                UserId = userId,
                Email = email,
                Role = role,
                Claims = claims
            });
            
        }
    }
}