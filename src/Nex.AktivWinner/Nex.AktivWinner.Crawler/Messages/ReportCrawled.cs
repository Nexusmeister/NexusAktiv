using MediatR;

namespace Nex.AktivWinner.Crawler.Messages;

public sealed class ReportCrawled : INotification
{
    public required Guid Id { get; init; }
    public required int SourceSystemId { get; set; }
    public required string ReportContent { get; init; }
}