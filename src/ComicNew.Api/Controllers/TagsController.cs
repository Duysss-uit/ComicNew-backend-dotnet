using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ComicNew.Application.DTOs.Tags;
using ComicNew.Application.Interfaces;

namespace ComicNew.Api.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly ILogger<TagsController> _logger;

    public TagsController(ITagService tagService, ILogger<TagsController> logger)
    {
        _tagService = tagService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTags(CancellationToken cancellationToken)
    {
        try
        {
            var tags = await _tagService.GetAllTagsAsync(cancellationToken);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tags");
            return StatusCode(500, new { message = "An error occurred while fetching tags." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTag(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var tag = await _tagService.GetTagByIdAsync(id, cancellationToken);
            if (tag == null) return NotFound();
            return Ok(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tag {Id}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the tag." });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tag = await _tagService.CreateTagAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tag");
            return StatusCode(500, new { message = "An error occurred while creating the tag." });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTag(Guid id, [FromBody] UpdateTagRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tag = await _tagService.UpdateTagAsync(id, request, cancellationToken);
            if (tag == null) return NotFound();
            return Ok(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tag {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the tag." });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTag(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _tagService.DeleteTagAsync(id, cancellationToken);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tag {Id}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the tag." });
        }
    }
}
