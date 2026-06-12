using ComicNew.Domain.Enums;
namespace ComicNew.Application.DTOs.Chapters;
public class CreateStoryRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? CoverUrl { get; set; } = string.Empty;
    public string[]? Tags { get; set; } = [];
    public StoryType Type { get; set; } = StoryType.Comic;
    public StoryStatus Status { get; set; } = StoryStatus.Ongoing;
}