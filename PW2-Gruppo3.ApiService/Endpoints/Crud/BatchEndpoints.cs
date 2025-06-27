using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.ApiService.Endpoints.Crud
{
    public static class BatchEndpoints
{
        public static IEndpointRouteBuilder MapBatchEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/v1/crud/batches");

            group.MapGet("/", GetAllBatches)
                .WithName("GetAllBatches")
                .WithOpenApi();

            group.MapGet("/{id}", GetBatchById)
                .WithName("GetBatchById")
                .WithOpenApi();

            group.MapPost("/", CreateBatch)
                .WithName("CreateBatch")
                .WithOpenApi();

            group.MapPut("/{id}", UpdateBatch)
                .WithName("UpdateBatch")
                .WithOpenApi();

            group.MapDelete("/{id}", DeleteBatch)
                .WithName("DeleteBatch")
                .WithOpenApi();

            return builder;
        }

        private static async Task<IResult> GetAllBatches(IGenericService<Batch> batchService)
        {
            var batches = await batchService.GetAllAsync();
            return Results.Ok(batches);
        }

        private static async Task<IResult> GetBatchById(Guid id, IGenericService<Batch> batchService)
        {
            var batch = await batchService.GetByIdAsync(id);
            return batch is null ? Results.NotFound() : Results.Ok(batch);
        }

        private static async Task<IResult> CreateBatch(Batch batch, IGenericService<Batch> batchService, BatchQueueService batchQueueService)
        {
            await batchQueueService.EnqueueAsync(batch.Id);
            
            await batchService.InsertAsync(batch);

            return Results.Ok(batch);
        }

        private static async Task<IResult> UpdateBatch(Guid id, Batch batch, IGenericService<Batch> batchService)
        {
            if (id != batch.Id)
                return Results.BadRequest("L'ID nella URL non corrisponde all'ID dell'oggetto");

            var existingBatch = await batchService.GetByIdAsync(id);

            if (existingBatch is null)
            {
                return Results.NotFound();
            }

            await batchService.UpdateAsync(batch);

            return Results.NoContent();
        }

        private static async Task<IResult> DeleteBatch(Guid id, IGenericService<Batch> batchService)
        {
            var batch = await batchService.GetByIdAsync(id);

            if (batch is null)
                return Results.NotFound();

            return Results.NoContent();
        }
    }
}
