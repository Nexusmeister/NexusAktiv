namespace AktivReader;

public interface IReaderService
{
    Task<List<PlayerData>> ReadDataAsync(CancellationToken token = default);
}