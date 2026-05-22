using System;
using System.Collections.Generic;
using ComicNew.Domain.Common;

namespace ComicNew.Domain.Entities;

public class ReadingHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid StoryId { get; set; }
    public int ChapterNumber { get; set; }
    public DateTime ReadAt { get; set; }
    public Story Story { get; set; } = null!;
    public User User { get; set; } = null!;
}