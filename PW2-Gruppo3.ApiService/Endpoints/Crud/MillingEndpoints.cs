using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.ApiService.Endpoints.Crud
{
    public static class MillingEndpoints
    {
        public static IEndpointRouteBuilder MapMillingEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/v1/crud/millings");

            group.MapGet("/", GetAllMillings)
                .WithName("GetAllMillings")
                .WithOpenApi();

            group.MapGet("/{id}", GetMillingById)
                .WithName("GetMillingById")
                .WithOpenApi();

            group.MapPost("/", CreateMilling)
                .WithName("CreateMilling")
                .WithOpenApi();

            group.MapPut("/{id}", UpdateMilling)
                .WithName("UpdateMilling")
                .WithOpenApi();

            group.MapDelete("/{id}", DeleteMilling)
                .WithName("DeleteMilling")
                .WithOpenApi();

            return builder;
        }

        private static async Task<IResult> GetAllMillings(IGenericService<Milling> millingService)
        {
            var millings = await millingService.GetAllAsync();

            return Results.Ok(millings);
        }

        private static async Task<IResult> GetMillingById(Guid id, IGenericService<Milling> millingService)
        {
            var milling = await millingService.GetByIdAsync(id);

            return Results.Ok(milling);
        }

        private static async Task<IResult> CreateMilling(Milling milling, IGenericService<Milling> millingService)
        {
            await millingService.InsertAsync(milling);

            return Results.Created($"/{milling.Id}", milling);
        }

        private static async Task<IResult> UpdateMilling(Guid id, Milling milling, IGenericService<Milling> millingService)
        {
            if (id != milling.Id)
                return Results.BadRequest("L'ID nella URL non corrisponde all'ID dell'oggetto");

            var existingMilling = await millingService.GetByIdAsync(id);

            if (existingMilling is null)
                return Results.NotFound();

            await millingService.UpdateAsync(milling);

            return Results.NoContent();
        }

        private static async Task<IResult> DeleteMilling(Guid id, IGenericService<Milling> millingService)
        {
            var milling = await millingService.GetByIdAsync(id);

            if (milling is null)
                return Results.NotFound();

            await millingService.DeleteAsync(id);

            return Results.NoContent();
        }
    }
}
