using MediatR;
using Nex.AktivWinner.Crawler.Entities;

namespace Nex.AktivWinner.Crawler.Messages;

public class ReportRead : INotification
{
    public required Guid Id { get; set; }
    public required int SourceSystemId { get; set; }
    public required IReadOnlyList<ReaderResult> MatchInformation { get; set; }
}