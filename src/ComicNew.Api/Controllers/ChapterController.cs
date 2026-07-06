using Microsoft.AspNetCore.Mvc;
using ComicNew.Application.DTOs.Chapters;
using ComicNew.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ComicNew.Application.DTOs.Stories;
namespace ComicNew.Api.Controllers
{
    [ApiController]
    [Route("api/stories/{storyId}/chapters")]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;
        private readonly IUserSyncService _userSyncService;
        private readonly IStoryService _storyService;
        private readonly IChapterUploadService _chapterUploadService;
        private readonly ILogger<ChapterController> _logger;

        public ChapterController(IChapterService chapterService, ILogger<ChapterController> logger, IUserSyncService userSyncService, IStoryService storyService, IChapterUploadService chapterUploadService)
        {
            _chapterService = chapterService;
            _logger = logger;   
            _userSyncService = userSyncService;
            _storyService = storyService;
            _chapterUploadService = chapterUploadService;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadChapter(Guid storyId, [FromForm] CreateChapterRequest request, List<IFormFile> files, CancellationToken cancellationToken)
        {
            try
            {
                if (request.StoryId != Guid.Empty && request.StoryId != storyId)
                {
                    return BadRequest(new { message = "StoryId in form data does not match the route storyId." });
                }

                request.StoryId = storyId;
                var user = await _userSyncService.GetOrCreateUserAsync(User);
                var story = await _storyService.GetStoryByIdAsync(storyId);
                if(story == null)
                {
                    return NotFound(new { message = "Story not found." });
                }
                if( story.Author == null || story.Author.Id != user.Id )
                {
                    return Forbid();
                }
                var uploadResult = await _chapterUploadService.UploadChapterAsync(storyId, request.ChapterNumber, request.Title, story.Type, files, cancellationToken);
                request.ImageUrls = uploadResult.ImageUrls;
                request.Content = uploadResult.Content;
                var chapter = await _chapterService.CreateChapterAsync(request);
                return CreatedAtAction(nameof(GetChapter), new { storyId, chapterId = chapter.Id }, chapter);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error uploading chapter");
                return StatusCode(500, new { message = "An error occurred while uploading the chapter." });
            }
        }
        [HttpGet("{chapterId}")]
        public async Task<IActionResult> GetChapter(Guid storyId, Guid chapterId)
        {
            try{
                var chapter = await _chapterService.GetChapterByIdAsync(chapterId);
                if (chapter == null)
                {
                    return NotFound(new { message = "Chapter not found." });
                }
                if (chapter.StoryId != storyId)
                {
                    return NotFound(new { message = "Chapter not found." });
                }
                await _chapterService.IncreaseView(storyId);
                return Ok(chapter);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching chapter {ChapterId}", chapterId);
                return StatusCode(500, new { message = "An error occurred while fetching the chapter." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetChaptersByStory(Guid storyId)
        {
            try
            {
                var chapters = await _chapterService.GetChapterByStoryIdAsync(storyId);
                return Ok(chapters);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching chapters for story {StoryId}", storyId);
                return StatusCode(500, new { message = "An error occurred while fetching the chapters." });
            }
        }
        [HttpDelete("{chapterId}")]
        [Authorize]
        public async Task<IActionResult> DeleteChapter(Guid storyId, Guid chapterId)
        {
            try
            {
                var chapter = await _chapterService.GetChapterByIdAsync(chapterId);
                if (chapter == null)
                {
                    return NotFound(new { message = "Chapter not found." });
                }
                if (chapter.StoryId != storyId)
                {
                    return NotFound(new { message = "Chapter not found." });
                }
                var user = await _userSyncService.GetOrCreateUserAsync(User);
                var story = await _storyService.GetStoryByIdAsync(chapter.StoryId);
                if(story == null)
                {
                    return NotFound(new { message = "Story not found." });
                }
                if(story.Author == null || story.Author.Id != user.Id)
                {
                    return Forbid();
                }
                await _chapterService.DeleteChapterAsync(chapterId);
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting chapter {ChapterId}", chapterId);
                return StatusCode(500, new { message = "An error occurred while deleting the chapter." });
            }
        }
    }
}