namespace AktivCrawler;

public class CrawlerOptions
{
    public string ExportPath { get; set; }
    public string BaseUri { get; set; }
    public int MaxThreads { get; set; }
    public int DelayInMs { get; set; }
}