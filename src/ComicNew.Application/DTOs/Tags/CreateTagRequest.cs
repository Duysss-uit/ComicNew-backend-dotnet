namespace ComicNew.Application.DTOs.Tags;

public class CreateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
}
