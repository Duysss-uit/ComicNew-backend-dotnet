namespace ComicNew.Application.DTOs.ReadingHistory;
public class AddReadingHistoryRequest
{
    public Guid UserId { get; set; }
    public Guid StoryId { get; set; }
    public int ChapterNumber { get; set; }
}