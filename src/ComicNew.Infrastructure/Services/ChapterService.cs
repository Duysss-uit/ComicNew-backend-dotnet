using ComicNew.Application.Interfaces;
using ComicNew.Application.DTOs.Chapters;
using ComicNew.Domain.Entities;
using ComicNew.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace ComicNew.Infrastructure.Services;

public class ChapterService : IChapterService
{
    private readonly AppDbContext _db;
    public ChapterService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<Chapter> CreateChapterAsync(CreateChapterRequest request, CancellationToken cancellationToken = default)
    {
        var newChapter = new Chapter
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            ChapterNumber = request.ChapterNumber,
            ImageUrls = request.ImageUrls ?? new List<string>(),
            StoryId = request.StoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.Chapters.Add(newChapter);
        await _db.SaveChangesAsync(cancellationToken);
        return newChapter;
    }

    public async Task DeleteChapterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chapter = await _db.Chapters.FindAsync(new object[] { id }, cancellationToken);
        if (chapter == null)
        {
            throw new Exception("Chapter not found");
        }
        _db.Chapters.Remove(chapter);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Chapter?> GetChapterByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Chapters.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<List<Chapter>> GetChapterByStoryIdAsync(Guid storyId, CancellationToken cancellationToken = default)
    {
        return await _db.Chapters.Where(c => c.StoryId == storyId).ToListAsync(cancellationToken);
    }
}