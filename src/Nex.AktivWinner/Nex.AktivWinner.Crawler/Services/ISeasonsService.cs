using Nex.AktivWinner.Data.Models;

namespace Nex.AktivWinner.Crawler.Services;

public interface ISeasonsService
{
    Task<Season?> InsertSeasonForMatchdate(DateOnly refDate, CancellationToken cancellationToken = default);
}