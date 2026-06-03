using System;
using System.Collections.Generic;
using ComicNew.Domain.Common;
using ComicNew.Domain.Entities;

namespace ComicNew.Domain.Entities;

public class User : BaseEntity
{
    public Guid? SupabaseUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Role { get; set; } = "user";
    public DateTime LastLoginAt { get; set; }
    public ICollection<Story> Stories { get; set; } = new List<Story>();
    public ICollection<ReadingHistory> ReadingHistory { get; set; } = new List<ReadingHistory>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
