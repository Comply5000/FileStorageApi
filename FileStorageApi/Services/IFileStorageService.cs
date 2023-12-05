using FileStorageApi.Models;

namespace FileStorageApi.Services;

public interface IFileStorageService
{
    Task<FileResponse> UploadAsync(IFormFile file, CancellationToken cancellationToken);
    Task<MemoryStream> DownloadAsync(byte[] data, CancellationToken cancellationToken);
}