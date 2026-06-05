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

            var userId = User.FindFirstValue("sub")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? syncedUser?.SupabaseUserId?.ToString()
                ?? syncedUser?.Id.ToString();
            var email = User.FindFirstValue("email")
                ?? User.FindFirstValue(ClaimTypes.Email)
                ?? syncedUser?.Email;
            var role = User.FindFirstValue("role") ?? syncedUser?.Role;
            var name = syncedUser?.FullName
                ?? User.FindFirstValue("full_name")
                ?? User.FindFirstValue("name")
                ?? email?.Split('@')[0]
                ?? "User";

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
                Claims = claims
            });
            
        }
    }
}
