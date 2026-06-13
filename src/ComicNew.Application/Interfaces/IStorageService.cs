namespace ComicNew.Application.Interfaces;
public interface IStorageService
{
    Task<string> UploadAsync(Stream fileStream,string fileName, string bucket, string folder,CancellationToken cancellationToken = default);
    Task DeleteAsync(string filePath, string bucket, CancellationToken cancellationToken = default);
}