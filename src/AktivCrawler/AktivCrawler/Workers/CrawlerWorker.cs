using System.Threading.Channels;
using AktivCrawler.Messages;
using AktivCrawler.Options;
using AktivCrawler.Services;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AktivCrawler.Workers;

public class CrawlerWorker : BackgroundService
{
    private readonly ILogger<CrawlerWorker> _logger;
    private readonly IOptions<CrawlerOptions> _options;
    private readonly IOptions<FilesOptions> _fileoptions;
    private readonly ICrawlerService _crawler;
    private readonly IFileManagerService _fileManager;
    private readonly IMediator _mediator;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CrawlerWorker(
        ILogger<CrawlerWorker> logger,
        IOptions<CrawlerOptions> options,
        IOptions<FilesOptions> fileoptions,
        ICrawlerService crawler,
        IFileManagerService fileManager,
        IMediator mediator)
    {
        _logger = logger;
        _options = options;
        _fileoptions = fileoptions;
        _crawler = crawler;
        _fileManager = fileManager;
        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var idToSearch = _options.Value.IdToStart;
        var taskList = new List<Task>();

        while (!stoppingToken.IsCancellationRequested)
        {
            
            for (var i = 0; i < _options.Value.MaxThreads; i++)
            {
                var search = idToSearch;

                var task = Task.Run(async () =>
                {
                    var filename = string.Concat("spielbericht_", search.ToString(), ".pdf");
                    if (!_fileManager.FileExists(Path.Combine(_fileoptions.Value.ArchivePath, filename)))
                    {
                        var stream = await _crawler.RequestMatchReportAsync(search, stoppingToken);

                        if (stream is not null)
                        {
                            await _fileManager.SaveStreamAsFile(_fileoptions.Value.ArchivePath, filename, stream, stoppingToken);
                            await _mediator.Publish(new ReportCrawled
                            {
                                Id = Guid.NewGuid(),
                                FileCreatedPath = filename
                            }, stoppingToken);
                        }
                    }
                }, stoppingToken);
                taskList.Add(task);

                idToSearch++;
            }

            await Task.WhenAll([.. taskList]);
            taskList.Clear();
        }
         
    }
}