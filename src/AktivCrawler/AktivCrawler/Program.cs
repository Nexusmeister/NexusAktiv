// See https://aka.ms/new-console-template for more information

using AktivCrawler;
using AktivCrawler.Messages;
using AktivCrawler.Options;
using AktivCrawler.Services;
using AktivCrawler.Workers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args);

host.ConfigureServices((hostingContext, services) =>
{
    services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssemblyContaining<Program>();
    });

    services.AddHttpClient<ICrawlerService, CrawlerService>((sp, client) =>
    {
        client.BaseAddress = new Uri(hostingContext.Configuration["Aktiv:BaseUri"]);
        client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        client.DefaultRequestHeaders.Add("Accept-Language", "de,de-DE;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
        client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        client.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Chromium\";v=\"122\", \"Not(A:Brand\";v=\"24\", \"Google Chrome\";v=\"122\"");
        client.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
        client.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
        client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
        
    });


    services.Configure<CrawlerOptions>(hostingContext.Configuration.GetSection(CrawlerOptions.Options))
        .Configure<FilesOptions>(hostingContext.Configuration.GetSection(FilesOptions.Options));

    services.AddTransient<IFileManagerService, FileManagerService>();
    services.AddTransient<IFileReaderService, PdfReaderService>();
    services.AddTransient<ITextToEntitiesService, TextToEntitiesService>();

    services.AddHostedService<CrawlerWorker>();
    //services.AddSingleton<ReaderWorker>();
    //services.AddSingleton<INotificationHandler<ReportCrawled>>(provider => provider.GetRequiredService<ReaderWorker>());
    //services.AddHostedService(provider => provider.GetRequiredService<ReaderWorker>()); // Register as hosted service
}).ConfigureAppConfiguration(config =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    
}).ConfigureLogging((hostingContext, logging) =>
{
    if (hostingContext.HostingEnvironment.IsDevelopment())
    {
        logging.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace);
    }
    else
    {
        logging.SetMinimumLevel(LogLevel.Information);
    }
});

var buildedHost = host.Build();
await buildedHost.RunAsync();