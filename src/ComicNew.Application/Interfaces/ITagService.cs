using ComicNew.Application.DTOs.Tags;

namespace ComicNew.Application.Interfaces;

public interface ITagService
{
    Task<List<TagDto>> GetAllTagsAsync(CancellationToken cancellationToken = default);
    Task<TagDto?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TagDto> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken = default);
    Task<TagDto?> UpdateTagAsync(Guid id, UpdateTagRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteTagAsync(Guid id, CancellationToken cancellationToken = default);
}
