using ComicNew.Domain.Entities;
namespace ComicNew.Application.DTOs.Chapters;
public class CreateChapterRequest
{
    public string Title { get; set; } = string.Empty;
    public int ChapterNumber { get; set; }
    public List<string>? ImageUrls { get; set; } = new List<string>();
    public Guid StoryId { get; set; } = Guid.Empty;
    public string? Content { get; set; }
}