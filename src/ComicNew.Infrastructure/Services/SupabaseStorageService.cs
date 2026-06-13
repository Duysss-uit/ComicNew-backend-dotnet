using ComicNew.Application.Interfaces;
using Supabase.Storage;
namespace ComicNew.Infrastructure.Services
{
    public class SupabaseStorageService : IStorageService
    {
        private readonly Supabase.Client _supabaseClient;
        public SupabaseStorageService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        public async Task<string> UploadAsync(Stream fileStream, string fileName, string bucket, string folder, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
            var bytes = memoryStream.ToArray();
            var extension = Path.GetExtension(fileName);
            var filePath = $"{folder}/{Guid.NewGuid()}{extension}";
            await _supabaseClient.Storage.From(bucket).Upload(bytes, filePath);
            return _supabaseClient.Storage.From(bucket).GetPublicUrl(filePath).ToString();
        }
        public async Task DeleteAsync(string filePath, string bucket, CancellationToken cancellationToken = default)
        {
            await _supabaseClient.Storage
                .From(bucket)
                .Remove(new List<string> { filePath });
        }
    }
}