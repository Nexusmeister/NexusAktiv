﻿using NexusCrawler.Domain.Models;

namespace AktivCrawler.Services;

public interface ISeasonsService
{
    Task<Season?> InsertSeasonForMatchdate(DateOnly refDate, CancellationToken cancellationToken = default);
}