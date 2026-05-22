using System;
using System.Collections.Generic;
using ComicNew.Domain.Common;

namespace ComicNew.Domain.Entities;

public class Story : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoverUrl { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
    public string Status { get; set; } = string.Empty;
    public int Views { get; set; }
    public double Rating { get; set; }
    public DateTime LastChapterAt { get; set; }

    // Relationships
    public User Author { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}