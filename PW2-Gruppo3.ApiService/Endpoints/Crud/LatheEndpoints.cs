using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.ApiService.Endpoints.Crud
{
    public static class LatheEndpoints
    {
        public static IEndpointRouteBuilder MapLatheEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/v1/crud/lathes");

            group.MapGet("/", GetAllLathes)
                .WithName("GetAllLathes")
                .WithOpenApi();

            group.MapGet("/{id}", GetLatheById)
                .WithName("GetLatheById")
                .WithOpenApi();

            group.MapPost("/", CreateLathe)
                .WithName("CreateLathe")
                .WithOpenApi();

            group.MapPut("/{id}", UpdateLathe)
                .WithName("UpdateLathe")
                .WithOpenApi();

            group.MapDelete("/{id}", DeleteLathe)
                .WithName("DeleteLathe")
                .WithOpenApi();

            return builder;
        }

        private static async Task<IResult> GetAllLathes(IGenericService<Lathe> latheService)
        {
            var lathes = await latheService.GetAllAsync();

            return Results.Ok(lathes);
        }

        private static async Task<IResult> GetLatheById(Guid id, IGenericService<Lathe> latheService)
        {
            var lathe = await latheService.GetByIdAsync(id);

            return lathe is null ? Results.NotFound() : Results.Ok(lathe);
        }

        private static async Task<IResult> CreateLathe(Lathe lathe, IGenericService<Lathe> latheService)
        {
            await latheService.InsertAsync(lathe);

            return Results.Created($"/{lathe.Id}", lathe);
        }

        private static async Task<IResult> UpdateLathe(Guid id, Lathe lathe, IGenericService<Lathe> latheService)
        {
            if (id != lathe.Id)
                return Results.BadRequest("L'ID nella URL non corrisponde all'ID dell'oggetto");

            var existingLathe = latheService.GetByIdAsync(id);

            if (existingLathe is null)
                return Results.NotFound();

            await latheService.UpdateAsync(lathe);

            return Results.NoContent();
        }

        private static async Task<IResult> DeleteLathe (Guid id, IGenericService<Lathe> latheService)
        {
            var lathe = await latheService.GetByIdAsync(id);
            
            if (lathe is null)
                return Results.NotFound();

            return Results.NoContent();
        }
    }
}
