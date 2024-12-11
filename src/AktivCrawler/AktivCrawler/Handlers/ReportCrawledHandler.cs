using AktivCrawler.Messages;
using AktivCrawler.Workers;
using MediatR;

namespace AktivCrawler.Handlers;

//public class ReportCrawledHandler //: INotificationHandler<ReportCrawled>
//{
//    public Task Handle(ReportCrawled notification, CancellationToken cancellationToken)
//    {
//        ReaderWorker._todo.Add(notification);
//        _todo.Add(notification);
//        _logger.LogInformation("{guid} / {filename} added to queue for read process", notification.Id, notification.FileCreatedPath);
//        return Task.CompletedTask;
//    }
//}