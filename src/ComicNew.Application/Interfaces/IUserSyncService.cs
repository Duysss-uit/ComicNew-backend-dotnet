using ComicNew.Domain.Entities;
using System.Security.Claims;

namespace ComicNew.Application.Interfaces;

public interface IUserSyncService
{
    Task<User> GetOrCreateUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
}