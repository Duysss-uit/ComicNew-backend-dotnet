namespace ComicNew.Application.DTOs.Tags;

public class UpdateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
}
