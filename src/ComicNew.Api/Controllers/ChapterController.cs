using Microsoft.AspNetCore.Mvc;
using ComicNew.Application.DTOs.Chapters;
using ComicNew.Application.Interfaces;
using ComicNew.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;
namespace ComicNew.Api.Controllers
{
    [ApiController]
    [Route("api/chapters")]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;
        private readonly IUserSyncService _userSyncService;
        private readonly IStorageService _storageService;
        private readonly IStoryService _storyService;
        private readonly IChapterUploadService _chapterUploadService;
        private readonly ILogger<ChapterController> _logger;

        public ChapterController(IChapterService chapterService, ILogger<ChapterController> logger, IStorageService storageService, IUserSyncService userSyncService, IStoryService storyService, IChapterUploadService chapterUploadService)
        {
            _chapterService = chapterService;
            _logger = logger;
            _storageService = storageService;
            _userSyncService = userSyncService;
            _storyService = storyService;
            _chapterUploadService = chapterUploadService;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadChapter([FromForm] CreateChapterRequest request, List<IFormFile> files, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userSyncService.GetOrCreateUserAsync(User);
                var story = await _storyService.GetStoryByIdAsync(request.StoryId);
                if(story == null)
                {
                    return NotFound(new { message = "Story not found." });
                }
                if(story.AuthorId != user.Id)
                {
                    return Forbid();
                }
                var uploadResult = await _chapterUploadService.UploadChapterAsync(request.StoryId, request.ChapterNumber, request.Title, story.Type, files, cancellationToken);
                request.ImageUrls = uploadResult.ImageUrls;
                request.Content = uploadResult.Content;
                var chapter = await _chapterService.CreateChapterAsync(request);
                return CreatedAtAction(nameof(UploadChapter), new { id = chapter.Id }, chapter);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error uploading chapter");
                return StatusCode(500, new { message = "An error occurred while uploading the chapter." });
            }
        }
    }
}