using System.ComponentModel;
using Microsoft.Extensions.Hosting;

namespace AktivReader;

public class ReaderWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            
        }
    }

    
}