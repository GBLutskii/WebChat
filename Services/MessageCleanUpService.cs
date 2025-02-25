using WebChatV1.DAL;

namespace WebChatV1.Services
{
    public class MessageCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageCleanupService> _logger;

        public MessageCleanupService(IServiceProvider serviceProvider, ILogger<MessageCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<RootContext>();
                        var threshold = DateTime.UtcNow.AddDays(-100);
                        var oldMessages = context.Messages
                            .Where(m => m.TimeSent < threshold)
                            .ToList();

                        if (oldMessages.Any())
                        {
                            context.Messages.RemoveRange(oldMessages);
                            await context.SaveChangesAsync();
                            _logger.LogInformation("Removed {Count} old messages", oldMessages.Count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up old messages");
                }
                
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}