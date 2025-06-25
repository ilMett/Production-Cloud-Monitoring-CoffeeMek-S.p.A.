using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.DataGenerator.Template;
using PW2_Gruppo3.Models;
using TestLine = PW2_Gruppo3.Models.TestLine;

namespace PW2_Gruppo3.ApiService.Endpoints.Crud
{
    public static class TestLineEndpoints
{
        public static IEndpointRouteBuilder MapTestLineEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/v1/crud/testlines");

            group.MapGet("/", GetAllTestLines)
                .WithName("GetAllTestLines")
                .WithOpenApi();

            group.MapGet("/{id}", GetTestLineById)
                .WithName("GetTestLineById")
                .WithOpenApi();

            group.MapPost("/", CreateTestLine)
                .WithName("CreateTestLine")
                .WithOpenApi();

            group.MapPut("/{id}", UpdateTestLine)
                .WithName("UpdateTestLine")
                .WithOpenApi();

            group.MapDelete("/{id}", DeleteTestLine)
                .WithName("DeleteTestLine")
                .WithOpenApi();

            return builder;
        }

        private static async Task<IResult> GetAllTestLines(IGenericService<TestLine> testLineService)
        {
            var testLines = await testLineService.GetAllAsync();

            return Results.Ok(testLines);
        }

        private static async Task<IResult> GetTestLineById(Guid id, IGenericService<TestLine> testLineService)
        {
            var testLine = await testLineService.GetByIdAsync(id);

            return Results.Ok(testLine);
        }

        private static async Task<IResult> CreateTestLine(TestLine testLine, IGenericService<TestLine> testLineService)
        {
            await testLineService.InsertAsync(testLine);

            return Results.Created($"/{testLine.Id}", testLine);
        }

        private static async Task<IResult> UpdateTestLine(Guid id, TestLine testLine, IGenericService<TestLine> testLineService)
        {
            if (id != testLine.Id)
                return Results.BadRequest("L'ID nella URL non corrisponde all'ID dell'oggetto");

            var existingTestLine = await testLineService.GetByIdAsync(id);

            if (existingTestLine is null)
                return Results.NotFound();

            await testLineService.UpdateAsync(testLine);

            return Results.NoContent();
        }

        private static async Task<IResult> DeleteTestLine(Guid id, IGenericService<TestLine> testLineService)
        {
            var testLine = await testLineService.GetByIdAsync(id);

            if (testLine is null)
                return Results.NotFound();

            await testLineService.DeleteAsync(id);

            return Results.NoContent();
        }

    }
}
