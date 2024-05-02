using AktivCrawler.Entities;

namespace AktivCrawler.Services;

public interface ITextToEntitiesService
{
    List<ReaderResult> GetEntitiesFromText(string linesRead);
}