using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PW2_Gruppo3.ApiService.Services;
using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.ApiService.Crud;

public static class SiteEndpoint
{
    public static IEndpointRouteBuilder MapSiteEndpoint(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/v1/crud");   //  qui la string prefix non dovrebbe finire con "/sites"??

        group.MapGet("/sites", GetAllSites) //  invece che ripeterlo qui ogni volta? STUPIDO IDIOTA!
            .WithName("GetSites")
            .WithOpenApi();

        group.MapGet("/sites/{id}", GetSiteById)
            .WithName("GetSiteById")
            .WithOpenApi();

        group.MapPost("/sites", CreateSite)
            .WithName("CreateSite")
            .WithOpenApi();

        group.MapPut("/sites/{id}", UpdateSite)
            .WithName("UpdateSite")
            .WithOpenApi();

        group.MapDelete("/sites/{id}", DeleteSite)
            .WithName("DeleteSite")
            .WithOpenApi();

        return builder;
    }

    private static async Task<IResult> GetAllSites(IGenericService<Site> siteService)
    {
        var sites = await siteService.GetAllAsync();
        return Results.Ok(sites);
    }

    private static async Task<IResult> GetSiteById(Guid id, IGenericService<Site> siteService)
    {
        var site = await siteService.GetByIdAsync(id);
        return site is null ? Results.NotFound() : Results.Ok(site);
    }

    private static async Task<IResult> CreateSite(Site site, IGenericService<Site> siteService)
    {
        await siteService.InsertAsync(site);
        return Results.Created($"/api/v1/crud/sites/{site.Id}", site);
    }

    private static async Task<IResult> UpdateSite(Guid id, Site site, IGenericService<Site> siteService)
    {
        if (id != site.Id)
            return Results.BadRequest("L'ID nella URL non corrisponde all'ID dell'oggetto");

        var existingSite = await siteService.GetByIdAsync(id);
        if (existingSite is null)
            return Results.NotFound();

        await siteService.UpdateAsync(site);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteSite(Guid id, IGenericService<Site> siteService)
    {
        var site = await siteService.GetByIdAsync(id);
        if (site is null)
            return Results.NotFound();

        await siteService.DeleteAsync(id);
        return Results.NoContent();
    }
}