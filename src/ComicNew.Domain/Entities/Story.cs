using System;
using System.Collections.Generic;
using ComicNew.Domain.Common;
using ComicNew.Domain.Enums;

namespace ComicNew.Domain.Entities;

public class Story : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoverUrl { get; set; } = string.Empty;
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public string Status { get; set; } = StoryStatus.Ongoing.ToString();
    public int Views { get; set; }
    public double Rating { get; set; }
    public DateTime LastChapterAt { get; set; }
    public StoryType Type { get; set; } = StoryType.Comic;

    // Relationships
    public User Author { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}