using Microsoft.EntityFrameworkCore;
using PW2_Gruppo3.ApiService.Data;
using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.ApiService.Services;

public interface IBatchQueueService
{
    Task EnqueueAsync(Guid uuid);
    Task<Guid?> DequeueAsync(Guid uuid);
    Task<IEnumerable<Guid>> GetAllAsync();
    Task ReorderAsync(IEnumerable<Guid> newOrder);
    Task<int> GetCountAsync();
    Task InitializeFromBatchQueueAsync();
    Task<Guid?> GetFirstBatchUuidAsync();
}

public class BatchQueueService : IBatchQueueService
{
    private readonly ProductionMonitoringContext _context;
    
    //Limits the number of threads that can access a resource or pool of resources concurrently.
    private readonly SemaphoreSlim _semaphore = new(1);

    public BatchQueueService(ProductionMonitoringContext context)
    {
        _context = context;
    }
    
    public async Task InitializeFromBatchQueueAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var batchItems = await _context.BatchQueueItems
                .OrderBy(b => b.Position)
                .ToListAsync();

            foreach (var batchItem in batchItems)
            {
                var queueItem = new BatchQueueItem()
                {
                    Id = Guid.NewGuid(),
                    BatchUuid = batchItem.BatchUuid,
                    Position = batchItem.Position,
                    CreatedAt = batchItem.CreatedAt
                };

                _context.BatchQueueItems.Add(queueItem);
            }

            await _context.SaveChangesAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }


    public async Task EnqueueAsync(Guid uuid)
    {
        await _semaphore.WaitAsync();
        try
        {
            var maxPosition = await _context.BatchQueueItems
                .MaxAsync(q => (int?)q.Position) ?? 0;

            var queueItem = new BatchQueueItem()
            {
                Id = Guid.NewGuid(),
                BatchUuid = uuid,
                Position = maxPosition + 1,
                CreatedAt = DateTime.UtcNow
            };

            _context.BatchQueueItems.Add(queueItem);
            await _context.SaveChangesAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Guid?> DequeueAsync(Guid uuid)
    {
        await _semaphore.WaitAsync();
        try
        {
            var item = await _context.BatchQueueItems
                .FindAsync(uuid);

            if (item == null)
                return null;

            _context.BatchQueueItems.Remove(item);
            await _context.SaveChangesAsync();

            // Riorganizza le posizioni rimanenti
            var remainingItems = await _context.BatchQueueItems
                .OrderBy(q => q.Position)
                .ToListAsync();

            for (int i = 0; i < remainingItems.Count; i++)
            {
                remainingItems[i].Position = i + 1;
            }

            await _context.SaveChangesAsync();

            return item.BatchUuid;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<Guid>> GetAllAsync()
    {
        return await _context.BatchQueueItems
            .OrderBy(q => q.Position)
            .Select(q => q.BatchUuid)
            .ToListAsync();
    }

    public async Task ReorderAsync(IEnumerable<Guid> newOrder)
    {
        await _semaphore.WaitAsync();
        try
        {
            var currentItems = await _context.BatchQueueItems.ToListAsync();
            var newOrderList = newOrder.ToList();

            for (int i = 0; i < newOrderList.Count; i++)
            {
                var item = currentItems.FirstOrDefault(x => x.BatchUuid == newOrderList[i]);
                if (item != null)
                {
                    item.Position = i + 1;
                }
            }

            await _context.SaveChangesAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.BatchQueueItems.CountAsync();
    }
    
    public async Task<Guid?> GetFirstBatchUuidAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var firstBatchId = await _context.BatchQueueItems
                .OrderBy(q => q.Position)
                .Select(q => (Guid?)q.BatchUuid) 
                .FirstOrDefaultAsync();

            return firstBatchId;
        }
        finally
        {
            _semaphore.Release();
        }
    }

}