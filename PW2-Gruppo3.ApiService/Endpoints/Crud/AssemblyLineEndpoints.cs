using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.ApiService.Endpoints.Crud
{
    public static class AssemblyLineEndpoints
    {
        public static IEndpointRouteBuilder MapAssemblyLineEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/v1/crud/assemblylines");

            group.MapGet("/", GetAllAssemblyLines)
                .WithName("GetAllAssemblyLines")
                .WithOpenApi();

            group.MapGet("/{id}", GetAssemblyLineById)
                .WithName("GetAssemblyLineById")
                .WithOpenApi();

            group.MapPost("/", CreateAssemblyLine)
                .WithName("CreateAssemblyLine")
                .WithOpenApi();

            group.MapPut("/{id}", UpdateAssemblyLine)
                .WithName("UpdateAssemblyLine")
                .WithOpenApi();

            group.MapDelete("/{id}", DeleteAssemblyLine)
                .WithName("DeleteAssemblyLine")
                .WithOpenApi();

            return builder;
        }

        private static async Task<IResult> GetAllAssemblyLines(IGenericService<AssemblyLine> assemblyLineService)
        {
            var assemblyLines = await assemblyLineService.GetAllAsync();
            return Results.Ok(assemblyLines);
        }

        private static async Task<IResult> GetAssemblyLineById(Guid id, IGenericService<AssemblyLine> assemblyLineService)
        {
            var assemblyLine = await assemblyLineService.GetByIdAsync(id);
            return assemblyLine is null ? Results.NotFound(): Results.Ok(assemblyLine);
        }

        private static async Task<IResult> CreateAssemblyLine(AssemblyLine assemblyLine, IGenericService<AssemblyLine> assemblyLineService)
        {
            await assemblyLineService.InsertAsync(assemblyLine);
            return Results.Created($"/{assemblyLine.Id}", assemblyLine);
        }

        private static async Task<IResult> UpdateAssemblyLine(Guid id, AssemblyLine assemblyLine, IGenericService<AssemblyLine> assemblyLineService)
        {
            if (id != assemblyLine.Id)
                return Results.BadRequest("L'ID nella URL non corrisponde all'ID dell'oggetto");

            var existingAssemblyLine = assemblyLineService.GetByIdAsync(id);

            if (existingAssemblyLine is null)
                return Results.NotFound();

            await assemblyLineService.UpdateAsync(assemblyLine);

            return Results.NoContent();
        }

        private static async Task<IResult> DeleteAssemblyLine(Guid id, IGenericService<AssemblyLine> assemblyLineService)
        {
            var assemblyLine = await assemblyLineService.GetByIdAsync(id);

            if (assemblyLine is null)
                return Results.NotFound();

            await assemblyLineService.DeleteAsync(id);
            return Results.NoContent();
        }
    }
}
