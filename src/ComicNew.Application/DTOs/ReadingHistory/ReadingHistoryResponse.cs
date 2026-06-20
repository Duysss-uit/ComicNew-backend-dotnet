using ComicNew.Application.DTOs;
namespace ComicNew.Application.DTOs.ReadingHistory;
public class ReadingHistoryResponse : BaseDto
{
    public Guid UserId { get; set; }
    public Guid StoryId { get; set; }
    public int ChapterNumber { get; set; }
}