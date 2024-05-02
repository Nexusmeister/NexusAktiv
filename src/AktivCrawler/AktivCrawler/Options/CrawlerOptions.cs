namespace AktivCrawler.Options;

public class CrawlerOptions
{
    public const string Options = "Aktiv";

    public required string BaseUri { get; init; }
    public required int MaxThreads { get; init; }
    public required int DelayInMs { get; init; }
    public required int IdToStart { get; init; }
    public required string SubUriPath { get; init; }
}