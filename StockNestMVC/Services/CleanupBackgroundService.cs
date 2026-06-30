using StockNestMVC.Interfaces;

namespace StockNestMVC.Services;

public class CleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CleanupBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var sessionRepo = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
                var notificationRepo = scope.ServiceProvider.GetRequiredService <INotificationRepository>();

                await sessionRepo.DeleteExpiredSessions();
                await notificationRepo.DeleteOldReadNotifications();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await Task.Delay(
                TimeSpan.FromHours(24),
                stoppingToken);
        }
    }
}
