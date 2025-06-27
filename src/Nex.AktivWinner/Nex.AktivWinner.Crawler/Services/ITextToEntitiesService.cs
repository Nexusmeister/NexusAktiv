using Nex.AktivWinner.Crawler.Entities;

namespace Nex.AktivWinner.Crawler.Services;

public interface ITextToEntitiesService
{
    List<ReaderResult> GetEntitiesFromText(string linesRead);
}