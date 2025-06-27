using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nex.AktivWinner.Crawler.Options;

namespace Nex.AktivWinner.Crawler.Services;

public sealed class FileManagerService(
    ILogger<FileManagerService> logger,
    IOptions<FilesOptions> options) : IFileManagerService
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

    public bool ArchiveFile(Guid processId)
    {
        var basepath = options.Value.WorkingDirectory;
        var dirInfo = new DirectoryInfo(options.Value.WorkingDirectory);

        if (!dirInfo.Exists)
        {
            logger.LogWarning("Path {filePath} was not found!", options.Value.WorkingDirectory);
            return false;
        }

        var files = dirInfo.GetFiles($"*{processId.ToString()}*", SearchOption.AllDirectories);
        switch (files.Length)
        {
            case 0:
                logger.LogWarning("File for process {processId} was not found and cannot be archived", processId);
                return false;
            case > 1:
                logger.LogWarning("Multiple files with process {processId} was found. Check errors in the preceding processes.", processId);
                return false;
        }

        var file = files[0];
        if (!file.Exists)
        {
            logger.LogWarning("File for process {processId} was not found!", processId);
            return false;
        }

        Directory.CreateDirectory(options.Value.ArchivePath);
        file.MoveTo(Path.Combine(options.Value.ArchivePath, file.Name));
        return true;
    }

    public int GetCountOfFiles(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        var getFiles = directoryInfo.GetFiles();
        return getFiles.Count(x => x is { Exists: true, Attributes: not (FileAttributes.Directory or FileAttributes.Archive) });
    }
}