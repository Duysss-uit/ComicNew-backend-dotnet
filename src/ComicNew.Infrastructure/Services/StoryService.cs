using ComicNew.Infrastructure.Persistence;
using ComicNew.Application.Interfaces;
using ComicNew.Application.DTOs.Stories;
using ComicNew.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ComicNew.Application.DTOs.Users;
using ComicNew.Application.DTOs.Tags;
namespace ComicNew.Infrastructure.Services;
public class StoryService : IStoryService
{
    private readonly AppDbContext _db;
    private readonly IEmbeddingService _embeddingService;

    public StoryService(AppDbContext db, IEmbeddingService embeddingService)
    {
        _db = db;
        _embeddingService = embeddingService;
    }
    public async Task<StoryResponseDto> CreateStoryAsync(CreateStoryRequest request, Guid authorId, CancellationToken cancellationToken = default)
    {
        Story newStory = new Story
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            CoverUrl = request.CoverUrl ?? string.Empty,
            Type = request.Type,
            Status = request.Status.ToString(),
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        if (request.TagIds != null && request.TagIds.Any())
        {
            var tags = await _db.Tags.Where(t => request.TagIds.Contains(t.Id)).ToListAsync(cancellationToken);
            newStory.Tags = tags;
        }

        try 
        {
            var textToEmbed = $"{newStory.Title} {newStory.Description}";
            var embedding = await _embeddingService.GenerateEmbeddingAsync(textToEmbed, cancellationToken);
            newStory.Embedding = new Pgvector.Vector(embedding);
        }
        catch (Exception ex)
        {
            // Log the error but don't fail story creation if embedding fails
            // Depending on the requirement, you might want to retry later or throw
            Console.WriteLine($"Failed to generate embedding: {ex.Message}");
        }

        _db.Stories.Add(newStory);
        await _db.SaveChangesAsync(cancellationToken);
        var author = await _db.Users.FindAsync(authorId);
        return new StoryResponseDto
        {
            Id = newStory.Id,
            Title = newStory.Title,
            Description = newStory.Description,
            CoverUrl = newStory.CoverUrl,
            Tags = newStory.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Slug = t.Slug }).ToList(),
            Type = newStory.Type,
            Status = newStory.Status,
            Author = new AuthorDto
            {
                Id = newStory.AuthorId,
                FullName = newStory.Author.FullName ?? string.Empty,
                AvatarUrl = newStory.Author.AvatarUrl ?? string.Empty
            },
        };
    }

    public async Task<StoryResponseDto?> GetStoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var story = await _db.Stories.Include(s => s.Author).Include(s => s.Tags).FirstOrDefaultAsync(
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
            Tags = story.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Slug = t.Slug }).ToList(),
            Type = story.Type,
            Status = story.Status,
            Author = new AuthorDto
            {
                Id = story.AuthorId,
                FullName = story.Author.FullName ?? string.Empty,
                AvatarUrl = story.Author.AvatarUrl ?? string.Empty
            },
        };
    }

    public async Task<List<StoryResponseDto>> GetStoriesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var stories = await _db.Stories.Include(s => s.Author).Include(s => s.Tags).OrderByDescending(s => s.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return stories.Select(s => new StoryResponseDto
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            CoverUrl = s.CoverUrl,
            Tags = s.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Slug = t.Slug }).ToList(),
            Type = s.Type,
            Status = s.Status,
            Author = new AuthorDto
            {
                Id = s.AuthorId,
                FullName = s.Author.FullName ?? string.Empty,
                AvatarUrl = s.Author.AvatarUrl ?? string.Empty
            }
        }).ToList();
    }
    public async Task<List<StoryResponseDto>?> GetStoriesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var author = await _db.Users.FindAsync(authorId);
        if (author == null)        {
            return null;
        }
        var stories = await _db.Stories.Include(s => s.Author).Include(s => s.Tags).Where(s => s.AuthorId == authorId).OrderByDescending(s => s.CreatedAt).ToListAsync(cancellationToken);
        return stories.Select(s => new StoryResponseDto
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            CoverUrl = s.CoverUrl,
            Tags = s.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Slug = t.Slug }).ToList(),
            Type = s.Type,
            Status = s.Status,
            Author = new AuthorDto
            {
                Id = author.Id,
                FullName = author.FullName ?? string.Empty,
                AvatarUrl = author.AvatarUrl ?? string.Empty
            }
        }).ToList();
    }

    public async Task<List<StoryResponseDto>> SearchStoriesAsync(string query, int matchCount = 10, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<StoryResponseDto>();
        }

        // Generate embedding for the search query
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query, cancellationToken);
        var pgvectorEmbedding = new Pgvector.Vector(queryEmbedding);

        // Execute Hybrid Search via raw SQL
        var stories = await _db.Stories
            .FromSqlRaw(
                "SELECT * FROM hybrid_search_stories({0}, {1}, {2})",
                query,
                pgvectorEmbedding,
                matchCount
            )
            .Include(s => s.Author)
            .Include(s => s.Tags)
            .ToListAsync(cancellationToken);

        return stories.Select(s => new StoryResponseDto
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            CoverUrl = s.CoverUrl,
            Tags = s.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Slug = t.Slug }).ToList(),
            Type = s.Type,
            Status = s.Status,
            Author = new AuthorDto
            {
                Id = s.AuthorId,
                FullName = s.Author?.FullName ?? string.Empty,
                AvatarUrl = s.Author?.AvatarUrl ?? string.Empty
            }
        }).ToList();
    }
}