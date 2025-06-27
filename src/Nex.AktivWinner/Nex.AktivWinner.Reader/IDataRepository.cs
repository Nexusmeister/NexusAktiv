namespace Nex.AktivWinner.Reader;

public interface IDataRepository
{
    Task<bool> SaveItemsAsync(PlayerData player, CancellationToken token = default);
}