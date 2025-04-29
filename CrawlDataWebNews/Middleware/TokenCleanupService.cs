// "-----------------------------------------------------------------------
//  <copyright file="TokenCleanupService .cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Data.Entities.Auth;
using CrawlDataWebNews.Infrastructure.UnitOfWork;
using CrawlDataWebNews.Manufacture;

namespace CrawlDataWebNews.Middleware
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromHours(AppSettings.TimeToClearTokenByHours); // clear refresh token one times per day
        private readonly ILogger<TokenCleanupService> _logger;
        public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var expiredTokens = await unitOfWork.RefreshToken
                            .FindByAsyn(t => t.ExpiredAt <= DateTimeOffset.UtcNow);

                        if (expiredTokens.Any())
                        {
                            await unitOfWork.BulkDeleteAsync<RefreshToken>(expiredTokens);
                        }
                    }
                    await Task.Delay(_interval, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
