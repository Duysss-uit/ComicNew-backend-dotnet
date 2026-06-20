using ComicNew.Application.Interfaces;
using ComicNew.Infrastructure.Persistence;
using ComicNew.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ComicNew.Application.DTOs.ReadingHistory;
namespace ComicNew.Infrastructure.Services;
public class ReadingHistoryService : IReadingHistoryService
{
    private readonly AppDbContext _db;

    public ReadingHistoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddReadingHistoryAsync(AddReadingHistoryRequest request, CancellationToken cancellationToken = default)
    {
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
            .ToListAsync(cancellationToken);

        return readingHistories.Select(rh => new ReadingHistoryResponse
        {
            UserId = rh.UserId,
            StoryId = rh.StoryId,
            ChapterNumber = rh.ChapterNumber
        }).ToList();
    }
}