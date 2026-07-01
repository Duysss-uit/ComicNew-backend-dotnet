using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ComicNew.Application.Interfaces;
 
namespace ComicNew.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IUserSyncService _userSyncService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserSyncService userSyncService, ILogger<AuthController> logger)
        {
            _userSyncService = userSyncService;
            _logger = logger;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            ComicNew.Domain.Entities.User? syncedUser = null;

            try
            {
                syncedUser = await _userSyncService.GetOrCreateUserAsync(User, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "User sync failed while handling /api/auth/me.");
            }
            if(syncedUser == null)
            {
                _logger.LogWarning("User sync returned null while handling /api/auth/me.");
            }

            var userId = syncedUser?.Id.ToString();
            var email = syncedUser?.Email;
            var role = syncedUser?.Role;
            var name = syncedUser?.FullName?? "User";

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Token does not contain a valid user id." });
            }

            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new
            {
                UserId = userId,
                Email = email,
                Role = role,
                Name = name,
                AvatarUrl = syncedUser?.AvatarUrl,
            });
            
        }
    }
}
