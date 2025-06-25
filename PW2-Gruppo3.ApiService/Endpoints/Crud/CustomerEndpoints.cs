using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.Models;
using System.Runtime.CompilerServices;

namespace PW2_Gruppo3.ApiService.Crud
{
    public static class CustomerEndpoints
    {
        public static IEndpointRouteBuilder MapCustomerEndpoint(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/v1/crud/customers");

            group.MapGet("/", GetAllCustomers)
                .WithName("GetCustomers")
                .WithOpenApi();

            group.MapGet("/{id}", GetCustomerById)
                .WithName("GetCustomerById")
                .WithOpenApi();

            group.MapPost("/", CreateCustomer)
                .WithName("CreateCustomer")
                .WithOpenApi();

            group.MapPut("/{id}", UpdateCustomer)
                .WithName("UpdateCustomer")
                .WithOpenApi();

            group.MapDelete("/{id}", DeleteCustomer)
                .WithName("DeleteCustomer")
                .WithOpenApi();

            return builder;
        }

        private static async Task<IResult> GetAllCustomers(IGenericService<Customer> customerService)
        {
            var customers = await customerService.GetAllAsync();
            return Results.Ok(customers);
        }

        private static async Task<IResult> GetCustomerById(Guid id, IGenericService<Customer> customerService)
        {
            var customer = await customerService.GetByIdAsync(id);
            return customer is null ? Results.NotFound() : Results.Ok(customer);
        }

        private static async Task<IResult> CreateCustomer(Customer customer, IGenericService<Customer> customerService)
        {
            await customerService.InsertAsync(customer);
            return Results.Created($"/{customer.Id}", customer);
        }

        private static async Task<IResult> UpdateCustomer(Guid id, Customer customer, IGenericService<Customer> customerService)
        {
            if (id != customer.Id)
                return Results.BadRequest("L'ID nella URL non corrisponde all'ID dell'oggetto");

            var existingCustomer = await customerService.GetByIdAsync(id);

            if (existingCustomer is null)
                return Results.NotFound();

            await customerService.UpdateAsync(customer);

            return Results.NoContent();

        }

        private static async Task<IResult> DeleteCustomer(Guid id, IGenericService<Customer> customerService)
        {
            var customer = await customerService.GetByIdAsync(id);

            if (customer is null)
                return Results.NotFound();

            await customerService.DeleteAsync(id);
            return Results.NoContent();
        }
    }
}
