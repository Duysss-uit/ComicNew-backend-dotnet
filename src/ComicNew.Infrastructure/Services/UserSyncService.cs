using System.Security.Claims;
using ComicNew.Application.Interfaces;
using ComicNew.Domain.Entities;
using ComicNew.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ComicNew.Infrastructure.Services;

public class UserSyncService : IUserSyncService
{
    private readonly AppDbContext _db;

    public UserSyncService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User> GetOrCreateUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        var supabaseId = principal.FindFirst("sub")?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = principal.FindFirst("email")?.Value
            ?? principal.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = principal.FindFirst("user_metadata.full_name")?.Value
            ?? principal.FindFirst("full_name")?.Value
            ?? email
            ?? string.Empty;
        var avatarUrl = principal.FindFirst("user_metadata.avatar_url")?.Value
            ?? principal.FindFirst("avatar_url")?.Value
            ?? string.Empty;

        if (!Guid.TryParse(supabaseId, out var supabaseUserId))
        {
            throw new InvalidOperationException("Authenticated user is missing a valid Supabase user id.");
        }

        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.SupabaseUserId == supabaseUserId,
            cancellationToken);

        if (user is not null)
        {
            return user;
        }

        user = new User
        {
            SupabaseUserId = supabaseUserId,
            Email = email ?? string.Empty,
            FullName = fullName,
            AvatarUrl = avatarUrl,
            LastLoginAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return user;
    }
}
