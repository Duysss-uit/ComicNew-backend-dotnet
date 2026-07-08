using System;
using System.Collections.Generic;
using ComicNew.Domain.Common;

namespace ComicNew.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    
    // Relationships
    public ICollection<Story> Stories { get; set; } = new List<Story>();
}
