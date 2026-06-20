using ComicNew.Domain.Entities;
using ComicNew.Application.DTOs.ReadingHistory;
namespace ComicNew.Application.Interfaces;
public interface IReadingHistoryService
{
    Task AddReadingHistoryAsync(AddReadingHistoryRequest request, CancellationToken cancellationToken = default);
    Task<List<ReadingHistoryResponse>> GetReadingHistoryByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);  
}