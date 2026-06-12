using ComicNew.Infrastructure.Persistence;
using ComicNew.Application.Interfaces;
using ComicNew.Application.DTOs.Chapters;
using ComicNew.Domain.Entities;
namespace ComicNew.Infrastructure.Services;
public class StoryService : IStoryService
{
    private readonly AppDbContext _db;
    public StoryService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<Story> CreateStoryAsync(CreateStoryRequest request, Guid authorId, CancellationToken cancellationToken = default)
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
        return newStory;
    }

    public Task<Story?> GetStoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Story>> GetStoriesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}