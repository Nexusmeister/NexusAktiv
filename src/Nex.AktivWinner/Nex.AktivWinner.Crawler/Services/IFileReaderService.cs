namespace Nex.AktivWinner.Crawler.Services;

public interface IFileReaderService
{
    string ReadData(string filepath);
    string ReadData(Stream stream);
}