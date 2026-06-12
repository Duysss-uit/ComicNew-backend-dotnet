using ComicNew.Domain.Entities;
using ComicNew.Application.DTOs.Stories;
public interface IStoryService
{
    Task<Story> CreateStoryAsync(CreateStoryRequest request, Guid authorId, CancellationToken cancellationToken = default);
    Task<Story?> GetStoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Story>> GetStoriesAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}