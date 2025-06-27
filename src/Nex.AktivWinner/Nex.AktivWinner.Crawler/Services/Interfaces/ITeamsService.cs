using Nex.AktivWinner.Data.Models;

namespace Nex.AktivWinner.Crawler.Services.Interfaces;

public interface ITeamsService
{
    Task<Team?> GetTeamByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Team> InsertTeamAsync(string name, int? clubId, CancellationToken cancellationToken = default);
}