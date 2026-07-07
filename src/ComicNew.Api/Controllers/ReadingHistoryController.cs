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
        private readonly IUserSyncService _userSyncService;

        public ReadingHistoryController(IReadingHistoryService readingHistoryService, IUserSyncService userSyncService)
        {
            _readingHistoryService = readingHistoryService;
            _userSyncService = userSyncService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReadingHistory([FromBody] AddReadingHistoryRequest request, CancellationToken cancellationToken)
        {
            var user = await _userSyncService.GetOrCreateUserAsync(User, cancellationToken);
            if (user == null) return Unauthorized();

            request.UserId = user.Id;
            await _readingHistoryService.AddReadingHistoryAsync(request, cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetReadingHistoryByUserId(CancellationToken cancellationToken)
        {
            var user = await _userSyncService.GetOrCreateUserAsync(User, cancellationToken);
            if (user == null) return Unauthorized();

            var readingHistories = await _readingHistoryService.GetReadingHistoryByUserIdAsync(user.Id, cancellationToken);
            return Ok(readingHistories);
        }
    }
}