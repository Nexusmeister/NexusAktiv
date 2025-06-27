namespace Nex.AktivWinner.Reader;

public interface IReaderService
{
    Task<List<PlayerData>> ReadDataAsync(CancellationToken token = default);
}