using System;
using ComicNew.Domain.Common;

namespace ComicNew.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string CreatedByIp { get; set; } = string.Empty;
    public string RevokedByIp { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}