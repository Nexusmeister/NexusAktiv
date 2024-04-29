namespace AktivReader;

public class LocalDataRepository : IDataRepository
{
    public Task<bool> SaveItemsAsync(PlayerData player, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}