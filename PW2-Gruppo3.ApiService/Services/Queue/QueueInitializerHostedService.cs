namespace PW2_Gruppo3.ApiService.Services;

public class QueueInitializerHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public QueueInitializerHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var queueService = scope.ServiceProvider.GetRequiredService<IBatchQueueService>();
        
        if (queueService is BatchQueueService persistentQueue)
        {
            await persistentQueue.InitializeFromBatchQueueAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }   
}