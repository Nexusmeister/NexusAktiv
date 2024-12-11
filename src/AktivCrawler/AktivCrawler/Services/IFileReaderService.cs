using AktivCrawler.Entities;

namespace AktivCrawler.Services;

public interface IFileReaderService
{
    string ReadData(string filepath);
}