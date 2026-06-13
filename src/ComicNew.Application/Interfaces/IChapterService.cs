using ComicNew.Domain.Entities;
using ComicNew.Application.DTOs.Chapters;
namespace ComicNew.Application.Interfaces;
public interface IChapterService
{
    Task<Chapter?> GetChapterByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Chapter>> GetChapterByStoryIdAsync(Guid storyId, CancellationToken cancellationToken = default);
    Task<Chapter> CreateChapterAsync(CreateChapterRequest request, CancellationToken cancellationToken = default);
    Task DeleteChapterAsync(Guid id, CancellationToken cancellationToken = default);
}