using System;
using System.Collections.Generic;
using ComicNew.Domain.Common;

namespace ComicNew.Domain.Entities;

public class Chapter : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int ChapterNumber { get; set; }
    public List<string> ImageUrls { get; set; } = new List<string>();
    public int Views { get; set; }
    public DateTime PublishedAt { get; set; }
    public Story Story { get; set; } = null!;
    public Guid StoryId { get; set; }
    public string? Content { get; set; }
}