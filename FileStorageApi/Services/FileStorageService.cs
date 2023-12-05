using FileStorageApi.Models;

namespace FileStorageApi.Services;

public sealed class FileStorageService : IFileStorageService
{
    public async Task<FileResponse> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await using var fileStream = file.OpenReadStream();
            var buffer = new byte[1024];
            var totalBytes = file.Length;
            var bytesRead = 0L;
            while (bytesRead < totalBytes)
            {
                var bytes = await fileStream.ReadAsync(buffer, cancellationToken);
                bytesRead += bytes;
                await memoryStream.WriteAsync(buffer.AsMemory(0, bytes), cancellationToken);
                var progressPercent = (double)bytesRead / totalBytes;
            }

            var fileData = memoryStream.ToArray();
            var fileRecord = new FileResponse(fileData, file.ContentType);
            await memoryStream.DisposeAsync();
            return fileRecord;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<MemoryStream> DownloadAsync(byte[] data, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream(data);

        return stream;
    }
}