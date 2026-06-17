using ComicNew.Domain.Common;
using ComicNew.Application.DTOs.Stories;
namespace ComicNew.Application.DTOs.Chapters
{
    public class ChapterResponseDto : BaseDto 
    {
        public string Title { get; set; } = string.Empty;
        public int ChapterNumber { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public int Views { get; set; }
        public DateTime PublishedAt { get; set; }
        public StoryResponseDto Story { get; set; } = null!;
        public Guid StoryId { get; set; }
        public string? Content { get; set; }
    }
}