using System;
using System.Collections.Generic;
using ComicNew.Domain.Common;
using ComicNew.Domain.Enums;
using Pgvector;
using NpgsqlTypes;

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
    
    // Vector for Semantic Search (model: llama-nemotron-embed-vl-1b-v2)
    // Actually the dimension for this model might be 2048 or something else. We'll let EF Core handle it.
    public Vector? Embedding { get; set; }
    
    // TSVector for Full-Text Search
    public NpgsqlTsVector? SearchVector { get; set; }

    // Relationships
    public User Author { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}