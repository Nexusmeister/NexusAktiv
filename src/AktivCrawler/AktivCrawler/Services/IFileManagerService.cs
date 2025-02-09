namespace AktivCrawler.Services;

public interface IFileManagerService
{
    bool FileExists(string path);
    Task SaveStreamAsFile(string filePath, string fileName, Stream inputStream, Guid processId, CancellationToken token = default);
    bool ArchiveFile(Guid processId);
}