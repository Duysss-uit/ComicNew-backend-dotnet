using ComicNew.Application.DTOs;
using ComicNew.Application.DTOs.Stories;

namespace ComicNew.Application.DTOs.ReadingHistory;
public class ReadingHistoryResponse : BaseDto
{
    public Guid UserId { get; set; }
    public int ChapterNumber { get; set; }
    public DateTime LastReadAt { get; set; }
    public StoryResponseDto Story { get; set; } = null!;
}