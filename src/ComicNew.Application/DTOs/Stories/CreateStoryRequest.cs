using ComicNew.Domain.Enums;
namespace ComicNew.Application.DTOs.Stories;
public class CreateStoryRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? CoverUrl { get; set; } = string.Empty;
    public List<Guid>? TagIds { get; set; } = new();
    public StoryType Type { get; set; } = StoryType.Comic;
    public StoryStatus Status { get; set; } = StoryStatus.Ongoing;
}