using ComicNew.Domain.Entities;
using ComicNew.Application.DTOs.Stories;
public interface IStoryService
{
    Task<StoryResponseDto> CreateStoryAsync(CreateStoryRequest request, Guid authorId, CancellationToken cancellationToken = default);
    Task<StoryResponseDto?> GetStoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<StoryResponseDto>> GetStoriesAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}