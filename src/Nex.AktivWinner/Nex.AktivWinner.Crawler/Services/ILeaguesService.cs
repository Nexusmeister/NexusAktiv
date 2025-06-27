using Nex.AktivWinner.Data.Models;

namespace Nex.AktivWinner.Crawler.Services;

public interface ILeaguesService
{
    Task<League?> GetLeagueByNameAndMatchdateAsync(string leagueName, Season refSeason, CancellationToken cancellationToken = default);
    Task<League?> InsertLeagueForSeasonAsync(string leagueName, Season refSeason, CancellationToken cancellationToken = default);
}