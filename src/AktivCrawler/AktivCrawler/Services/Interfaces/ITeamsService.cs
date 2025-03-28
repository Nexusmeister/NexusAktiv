using NexusCrawler.Domain.Models;

namespace AktivCrawler.Services.Interfaces;

public interface ITeamsService
{
    Task<Team?> GetTeamByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Team> InsertTeamAsync(string name, int? clubId, CancellationToken cancellationToken = default);
}