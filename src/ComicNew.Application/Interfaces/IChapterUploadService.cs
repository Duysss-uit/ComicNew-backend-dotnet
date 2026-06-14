using ComicNew.Domain.Entities;
using ComicNew.Domain.Enums;
using Microsoft.AspNetCore.Http;
namespace ComicNew.Application.Interfaces;
public interface IChapterUploadService
{
    Task<ChapterUploadResult> UploadChapterAsync(
        Guid storyId,
        int chapterNumber,
        string title,
        StoryType storyType,
        List<IFormFile> files,
        CancellationToken cancellationToken = default);
}