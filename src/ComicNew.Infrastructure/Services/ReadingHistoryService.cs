using ComicNew.Application.Interfaces;
using ComicNew.Infrastructure.Persistence;
using ComicNew.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ComicNew.Application.DTOs.ReadingHistory;
namespace ComicNew.Infrastructure.Services;
public class ReadingHistoryService : IReadingHistoryService
{
    private readonly AppDbContext _db;
    private readonly IStoryService _storyService;

    public ReadingHistoryService(AppDbContext db, IStoryService storyService)
    {
        _db = db;
        _storyService = storyService;
    }
    public async Task AddReadingHistoryAsync(AddReadingHistoryRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _db.ReadingHistories
            .FirstOrDefaultAsync(rh => rh.UserId == request.UserId && rh.StoryId == request.StoryId, cancellationToken);
        if(existing != null)
        {
            existing.ChapterNumber = request.ChapterNumber;
            _db.ReadingHistories.Update(existing);
            await _db.SaveChangesAsync(cancellationToken);
            return;
        }
        var readingHistory = new ReadingHistory
        {
            UserId = request.UserId,
            StoryId = request.StoryId,
            ChapterNumber = request.ChapterNumber,
        };
        _db.ReadingHistories.Add(readingHistory);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ReadingHistoryResponse>> GetReadingHistoryByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var readingHistories = await _db.ReadingHistories
            .Where(rh => rh.UserId == userId)
            .OrderByDescending(rh => rh.UpdatedAt)
            .ToListAsync(cancellationToken);

        var responseList = new List<ReadingHistoryResponse>();

        foreach (var rh in readingHistories)
        {
            var story = await _storyService.GetStoryByIdAsync(rh.StoryId, cancellationToken);
            if (story == null)
            {
                continue;
            }

            responseList.Add(new ReadingHistoryResponse
            {
                UserId = rh.UserId,
                ChapterNumber = rh.ChapterNumber,
                LastReadAt = rh.UpdatedAt,
                Story = story
            });
        }

        return responseList;
    }
}