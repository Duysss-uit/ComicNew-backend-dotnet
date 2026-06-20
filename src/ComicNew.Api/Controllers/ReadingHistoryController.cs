using Microsoft.AspNetCore.Mvc;
using ComicNew.Application.Interfaces;
using ComicNew.Application.DTOs.ReadingHistory;
using Microsoft.AspNetCore.Authorization;
namespace ComicNew.Api.Controllers{
    [ApiController]
    [Route("api/user/reading-history")]
    [Authorize]
    public class ReadingHistoryController : ControllerBase
    {
        private readonly IReadingHistoryService _readingHistoryService;

        public ReadingHistoryController(IReadingHistoryService readingHistoryService)
        {
            _readingHistoryService = readingHistoryService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReadingHistory([FromBody] AddReadingHistoryRequest request, CancellationToken cancellationToken)
        {
            await _readingHistoryService.AddReadingHistoryAsync(request, cancellationToken);
            return Ok();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetReadingHistoryByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var readingHistories = await _readingHistoryService.GetReadingHistoryByUserIdAsync(userId, cancellationToken);
            return Ok(readingHistories);
        }
    }
}