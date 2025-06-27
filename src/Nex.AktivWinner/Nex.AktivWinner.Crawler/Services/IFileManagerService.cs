namespace Nex.AktivWinner.Crawler.Services;

public interface IFileManagerService
{
    bool FileExists(string path);
    bool FileExists(string path, string searchTerm);
    Task SaveStreamAsFile(string filePath, string fileName, Stream inputStream, Guid processId, CancellationToken token = default);
    int GetCountOfFiles(string path);
}