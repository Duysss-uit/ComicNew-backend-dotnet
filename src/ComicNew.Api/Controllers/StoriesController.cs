using Microsoft.AspNetCore.Mvc;
using ComicNew.Application.DTOs.Stories;
using ComicNew.Application.Interfaces;
using ComicNew.Application.Constants;
using Microsoft.AspNetCore.Authorization;
namespace ComicNew.Api.Controllers
{
    [ApiController]
    [Route("api/stories")]
    public class StoriesController : ControllerBase
    {
        private readonly IStoryService _storyService;
        private readonly ILogger<StoriesController> _logger;
        private readonly IStorageService _storageService;
        private readonly IUserSyncService _userSyncService;

        public StoriesController(IStoryService storyService, ILogger<StoriesController> logger, IStorageService storageService, IUserSyncService userSyncService)
        {
            _storyService = storyService;
            _storageService = storageService;
            _userSyncService = userSyncService;
            _logger = logger;
        }
        [HttpGet("{storyId}")]
        public async Task<IActionResult> GetStory(Guid storyId, CancellationToken cancellationToken)
        {
            try
            {
                var story = await _storyService.GetStoryByIdAsync(storyId, cancellationToken);
                if (story == null)
                {
                    return NotFound();
                }
                return Ok(story);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching story {StoryId}", storyId);
                return StatusCode(500, new { message = "An error occurred while fetching the story." });
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateStory([FromForm]CreateStoryRequest request, IFormFile? coverFile, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userSyncService.GetOrCreateUserAsync(User);
                if(coverFile != null)
                {
                    var coverUrl = await _storageService.UploadAsync(coverFile.OpenReadStream(), coverFile.FileName, StorageBuckets.Cover, "covers", cancellationToken);
                    request.CoverUrl = coverUrl;
                }
                var story = await _storyService.CreateStoryAsync(request, user.Id, cancellationToken);
                return CreatedAtAction(nameof(GetStory), new { storyId = story.Id }, story);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating story");
                return StatusCode(500, new { message = "An error occurred while creating the story." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetStories([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var stories = await _storyService.GetStoriesAsync(page, pageSize, cancellationToken);
                return Ok(stories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stories");
                return StatusCode(500, new { message = "An error occurred while fetching the stories." });
            }
        }
    }
}