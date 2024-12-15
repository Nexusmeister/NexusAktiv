using AktivCrawler.Entities;
using MediatR;

namespace AktivCrawler.Messages;

public class ReportRead : INotification
{
    public required Guid Id { get; set; }
    public required int SourceSystemId { get; set; }
    public required IReadOnlyList<ReaderResult> MatchInformation { get; set; }
}