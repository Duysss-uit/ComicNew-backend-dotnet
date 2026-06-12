using Microsoft.AspNetCore.Mvc;
using ComicNew.Application.DTOs.Stories;
using ComicNew.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace ComicNew.Api.Controllers
{
    [ApiController]
    [Route("api/stories")]
    public class StoriesController : ControllerBase
    {
        private readonly IStoryService _storyService;
        private readonly ILogger<StoriesController> _logger;

        public StoriesController(IStoryService storyService, ILogger<StoriesController> logger)
        {
            _storyService = storyService;
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
        public async Task<IActionResult> CreateStory(CreateStoryRequest request, IUserSyncService userSyncService, CancellationToken cancellationToken)
        {
            try
            {
                var story = await _storyService.CreateStoryAsync(request, Guid.NewGuid(), cancellationToken);
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