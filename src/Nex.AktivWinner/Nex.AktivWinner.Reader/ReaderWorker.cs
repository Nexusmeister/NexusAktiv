using Microsoft.Extensions.Hosting;

namespace Nex.AktivWinner.Reader;

public class ReaderWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            
        }
    }

    
}