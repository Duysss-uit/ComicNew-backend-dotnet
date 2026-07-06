using ComicNew.Domain.Entities;
using ComicNew.Application.DTOs.Chapters;
namespace ComicNew.Application.Interfaces;
public interface IChapterService
{
    Task<ChapterResponseDto?> GetChapterByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ChapterResponseDto>> GetChapterByStoryIdAsync(Guid storyId, CancellationToken cancellationToken = default);
    Task<ChapterResponseDto> CreateChapterAsync(CreateChapterRequest request, CancellationToken cancellationToken = default);
    Task DeleteChapterAsync(Guid id, CancellationToken cancellationToken = default);
    Task IncreaseView(Guid storyId, CancellationToken cancellationToken = default);
}