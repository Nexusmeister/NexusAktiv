using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nex.AktivWinner.Crawler.Messages;
using Nex.AktivWinner.Crawler.Options;
using Nex.AktivWinner.Crawler.Services;

namespace Nex.AktivWinner.Crawler.Workers;

public class CrawlerWorker : BackgroundService
{
    private readonly ILogger<CrawlerWorker> _logger;
    private readonly IOptions<CrawlerOptions> _options;
    private readonly ICrawlerService _crawler;
    private readonly IFileManagerService _fileManager;
    private readonly IFileReaderService _fileReader;
    private readonly IMediator _mediator;
    private readonly IOptionsMonitor<ShutdownRequestOptions> _shutdownOptions;
    private readonly IHostApplicationLifetime _applicationLifetime;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CrawlerWorker(
        ILogger<CrawlerWorker> logger,
        IOptions<CrawlerOptions> options,
        ICrawlerService crawler,
        IFileManagerService fileManager,
        IFileReaderService fileReader,
        IMediator mediator,
        IOptionsMonitor<ShutdownRequestOptions> shutdownOptions,
        IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _options = options;
        _crawler = crawler;
        _fileManager = fileManager;
        _fileReader = fileReader;
        _mediator = mediator;
        _shutdownOptions = shutdownOptions;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var idToSearch = _options.Value.IdToStart;
        var taskList = new List<Task>();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_shutdownOptions.CurrentValue.ShutdownRequested)
            {
                _logger.LogInformation("Stopping service and application as ShutdownRequest from appsettings got detected");
                _applicationLifetime.StopApplication();
                break;
            }

            if (idToSearch >= _options.Value.MaxSearchId)
            {
                _logger.LogInformation("All reports got processed. Shut down after a short pause");
                await Task.Delay(10000, stoppingToken);
                _applicationLifetime.StopApplication();
                break;
            }

            for (var i = 0; i < _options.Value.MaxThreads; i++)
            {
                var search = idToSearch;

                var task = Task.Run(async () =>
                {
                    // Every import process gets a unique identifier - will be stored as a reference
                    var processId = Guid.NewGuid();
                    var stream = await _crawler.RequestMatchReportAsync(search, stoppingToken);

                    if (stream is not null)
                    {
                        var content = _fileReader.ReadData(stream);

                        await _mediator.Publish(new ReportCrawled
                        {
                            Id = processId,
                            SourceSystemId = search,
                            ReportContent = content
                        }, stoppingToken);
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