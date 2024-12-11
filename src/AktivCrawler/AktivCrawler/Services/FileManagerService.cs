using Microsoft.Extensions.Logging;

namespace AktivCrawler.Services;

public class FileManagerService(ILogger<FileManagerService> logger) : IFileManagerService
{
    public bool FileExists(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        logger.LogInformation("{gamereportFile} already exists - Can be skipped", path);
        return true;
    }

    public async Task SaveStreamAsFile(string filePath, string fileName, Stream inputStream, CancellationToken token = default)
    {
        var path = Path.Combine(filePath, fileName);
        var bytesInStream = new byte[inputStream.Length];

        _ = await inputStream.ReadAsync(bytesInStream.AsMemory(0, bytesInStream.Length), token);

        var memory = new ReadOnlyMemory<byte>(bytesInStream);
        Directory.CreateDirectory(filePath);

        await using (var outputFileStream = new FileStream(path, FileMode.Create))
        {
            await outputFileStream.WriteAsync(memory, token);
        }

        logger.LogInformation("Saved {fileName}", fileName);
    }
}