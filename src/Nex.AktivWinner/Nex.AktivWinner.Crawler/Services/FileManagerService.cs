using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nex.AktivWinner.Crawler.Options;

namespace Nex.AktivWinner.Crawler.Services;

public sealed class FileManagerService(
    ILogger<FileManagerService> logger) : IFileManagerService
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

    public bool FileExists(string path, string searchTerm)
    {
        // First try the simple solution
        if (FileExists(Path.Combine(path, searchTerm)))
        {
            return true;
        }

        // Second try with the search term
        var basePath = new DirectoryInfo(path);

        // It is not important if the file got imported one or more times
        if (basePath.GetFiles($"*{searchTerm}*", SearchOption.AllDirectories).Length > 0)
        {
            logger.LogInformation("{gamereportFile} already exists - Can be skipped", path);
            return true;
        }

        return false;
    }

    public async Task SaveStreamAsFile(string filePath, string fileName, Stream inputStream, Guid processId, CancellationToken token = default)
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

    public int GetCountOfFiles(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        var getFiles = directoryInfo.GetFiles();
        return getFiles.Count(x => x is { Exists: true, Attributes: not (FileAttributes.Directory or FileAttributes.Archive) });
    }
}