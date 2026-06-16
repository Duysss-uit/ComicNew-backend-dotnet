using ComicNew.Infrastructure.Persistence;
using ComicNew.Application.Interfaces;
using ComicNew.Application.DTOs.Stories;
using ComicNew.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ComicNew.Application.DTOs.Users;
namespace ComicNew.Infrastructure.Services;
public class StoryService : IStoryService
{
    private readonly AppDbContext _db;
    public StoryService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<StoryResponseDto> CreateStoryAsync(CreateStoryRequest request, Guid authorId, CancellationToken cancellationToken = default)
    {
        Story newStory = new Story
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            CoverUrl = request.CoverUrl ?? string.Empty,
            Tags = request.Tags ?? new string[] { },
            Type = request.Type,
            Status = request.Status.ToString(),
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.Stories.Add(newStory);
        await _db.SaveChangesAsync(cancellationToken);
        return new StoryResponseDto
        {
            Id = newStory.Id,
            Title = newStory.Title,
            Description = newStory.Description,
            CoverUrl = newStory.CoverUrl,
            Tags = newStory.Tags,
            Type = newStory.Type,
            Status = newStory.Status,
            Author = new AuthorDto
            {
                Id = newStory.AuthorId,
                FullName = newStory.Author.FullName,
                AvatarUrl = newStory.Author.AvatarUrl
            },
        };
    }

    public async Task<StoryResponseDto?> GetStoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var story = await _db.Stories.FirstOrDefaultAsync(
            s => s.Id == id,
            cancellationToken);
        if (story == null)
        {
            return null;
        }
        return new StoryResponseDto
        {
            Id = story.Id,
            Title = story.Title,
            Description = story.Description,
            CoverUrl = story.CoverUrl,
            Tags = story.Tags,
            Type = story.Type,
            Status = story.Status,
            Author = new AuthorDto
            {
                Id = story.AuthorId,
                FullName = story.Author.FullName,
                AvatarUrl = story.Author.AvatarUrl
            },
        };
    }

    public async Task<List<StoryResponseDto>> GetStoriesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var stories = await _db.Stories.OrderByDescending(s => s.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return stories.Select(s => new StoryResponseDto
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            CoverUrl = s.CoverUrl,
            Tags = s.Tags,
            Type = s.Type,
            Status = s.Status,
            Author = new AuthorDto
            {
                Id = s.AuthorId,
                FullName = s.Author.FullName,
                AvatarUrl = s.Author.AvatarUrl
            }
        }).ToList();
    }
}