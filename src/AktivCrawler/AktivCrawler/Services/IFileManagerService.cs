namespace AktivCrawler.Services;

public interface IFileManagerService
{
    bool FileExists(string path);
    Task SaveStreamAsFile(string filePath, string fileName, Stream inputStream, CancellationToken token = default);
}