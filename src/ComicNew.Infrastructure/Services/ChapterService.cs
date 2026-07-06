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
    public async Task IncreaseView(Guid storyId, CancellationToken cancellationToken = default)
    {
        var story = await _db.Stories.FindAsync(new object[] { storyId }, cancellationToken);
        if (story == null)
        {
            throw new Exception("Story not found");
        }
        story.Views++;
        _db.Stories.Update(story);
        await _db.SaveChangesAsync(cancellationToken);
    }
    public async Task<ChapterResponseDto> CreateChapterAsync(CreateChapterRequest request, CancellationToken cancellationToken = default)
    {
        var newChapter = new Chapter
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            ChapterNumber = request.ChapterNumber,
            ImageUrls = request.ImageUrls ?? new List<string>(),
            Content = request.Content ?? string.Empty,
            StoryId = request.StoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
            
        };
        _db.Chapters.Add(newChapter);
        await _db.SaveChangesAsync(cancellationToken);
        return new ChapterResponseDto
        {
            Id = newChapter.Id,
            Title = newChapter.Title,
            ChapterNumber = newChapter.ChapterNumber,
            ImageUrls = newChapter.ImageUrls,
            Content = newChapter.Content,
            StoryId = newChapter.StoryId,
            CreatedAt = newChapter.CreatedAt,
            UpdatedAt = newChapter.UpdatedAt
        };
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

    public async Task<ChapterResponseDto?> GetChapterByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chapter = await _db.Chapters.FindAsync(new object[] { id }, cancellationToken);
        if (chapter == null)
        {
            return null;
        }
        return new ChapterResponseDto
        {
            Id = chapter.Id,
            Title = chapter.Title,
            ChapterNumber = chapter.ChapterNumber,
            ImageUrls = chapter.ImageUrls,
            Content = chapter.Content,
            StoryId = chapter.StoryId,
            CreatedAt = chapter.CreatedAt,
            UpdatedAt = chapter.UpdatedAt
        };
    }
    public async Task<List<ChapterResponseDto>> GetChapterByStoryIdAsync(Guid storyId, CancellationToken cancellationToken = default)
    {
        var chapters = await _db.Chapters.Where(c => c.StoryId == storyId).ToListAsync(cancellationToken);
        return chapters.Select(c => new ChapterResponseDto
        {
            Id = c.Id,
            Title = c.Title,
            ChapterNumber = c.ChapterNumber,
            ImageUrls = c.ImageUrls,
            Content = c.Content,
            StoryId = c.StoryId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();
    }
}