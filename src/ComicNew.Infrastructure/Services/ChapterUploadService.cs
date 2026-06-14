using ComicNew.Application.Interfaces;
using ComicNew.Domain.Entities;
using ComicNew.Domain.Enums;
using Microsoft.AspNetCore.Http;
using ComicNew.Application.Constants;
using ComicNew.Application.DTOs.Chapters;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;
namespace ComicNew.Infrastructure.Services;
public class ChapterUploadService : IChapterUploadService
{
    private readonly IStorageService _storageService;
    public ChapterUploadService(IStorageService storageService, ChapterUploadResult chapterUploadResult)
    {
        _storageService = storageService;
    }
    async Task<List<string>> ComicUploadAsync(Guid storyId, int chapterNumber, string title, List<IFormFile> files, CancellationToken cancellationToken)
    {
        var urls = new List<string>();
        foreach(var file in files)
        {
                var url = await _storageService.UploadAsync(file.OpenReadStream(), file.FileName, StorageBuckets.Chapters, $"{storyId}/{chapterNumber}", cancellationToken);
                urls.Add(url);
        }
        return urls;
    }
    async Task<string> NovelUploadAsync(Guid storyId, int chapterNumber, string title, List<IFormFile> files, CancellationToken cancellationToken)
    {
        if(files.Count > 1)
        {
            throw new NotSupportedException(("Only one file is allowed for novel chapters."));
        }
        var file = files.First();
        if (file == null)
        {
            throw new NotSupportedException(("No file provided."));
        }
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension == ".docx")
        {
            using var stream = file.OpenReadStream();
            using var doc = WordprocessingDocument.Open(stream, false);
            var Content = doc.MainDocumentPart?.Document?.Body?.InnerText ?? string.Empty;
            if(string.IsNullOrWhiteSpace(Content))
            {
                throw new NotSupportedException(("The provided .docx file does not contain any text."));
            }
            return Content;
        }
        else if (extension == ".pdf")
        {
            using var stream = file.OpenReadStream();
            using var pdf = PdfDocument.Open(stream);
            var Content = string.Join("\n", pdf.GetPages().Select(p => p.Text));
            return Content;
        }
        else
        {
            throw new NotSupportedException(("Novel only supports .docx or .pdf files."));
        }
    }
    public async Task<ChapterUploadResult> UploadChapterAsync(Guid storyId,int chapterNumber,string title,StoryType storyType,List<IFormFile> files,CancellationToken cancellationToken = default){
        if(storyType == StoryType.Comic)
        {
            var urls = await ComicUploadAsync(storyId, chapterNumber, title, files, cancellationToken = default);
            return new ChapterUploadResult { ImageUrls = urls };
        }
        else 
        {
            var content = await NovelUploadAsync(storyId, chapterNumber, title, files, cancellationToken = default);
            return new ChapterUploadResult { Content = content };
        }
    }
}