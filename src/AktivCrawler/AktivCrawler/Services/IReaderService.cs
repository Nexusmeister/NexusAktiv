using AktivCrawler.Entities;

namespace AktivCrawler.Services;

public interface IReaderService
{
    string ReadData(string filepath);
}